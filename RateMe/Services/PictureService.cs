using System.IO;
using System.Windows.Media.Imaging;
using RateMe.Api.S3ServiceApi;
using RateMe.Utils.LocalHelpers;
using RateMeShared.Dto;

namespace RateMe.Services;

public class PictureService
{
    public bool IsServiceAlive { get; }
    
    private const string JpegMediaType = "image/jpeg"; 
    
    private PictureClient _pictureClient;
    
    public PictureService(PictureClient client, bool isServiceAlive)
    {
        IsServiceAlive = isServiceAlive;
        _pictureClient = client;
    }

    public async Task LoadPictureFromS3(string id)
    {
        string url = await _pictureClient.GetPresignedGetUrl(id);

        if (string.IsNullOrEmpty(url))
        {
            return;
        }

        MemoryStream? stream = await _pictureClient.GetDataViaPresignedUrl(url);

        if (stream != null)
        {
            await PictureHelper.GenerateProfilePictureFromStream(stream);
        }
    }
    
    public async Task<string?> UploadJpgPicture(string path)
    {
        PresignedUploadDto? preSignedDto = await _pictureClient.GetPreSignedUploadUrl();

        if (preSignedDto == null)
        {
            return null;
        }
        
        byte[] fileBytes = await File.ReadAllBytesAsync(path);
        await _pictureClient.PushDataViaPreSignedUrl(fileBytes, JpegMediaType, preSignedDto.Url);

        return preSignedDto.Id;
    }
    
    public async Task UpdateJpgPicture(string s3Id, string path)
    {
        string? presignedUrl = await _pictureClient.GetPreSignedUpdateUrl(s3Id);

        if (string.IsNullOrEmpty(presignedUrl))
        {
            return;
        }
        
        byte[] fileBytes = await File.ReadAllBytesAsync(path);
        await _pictureClient.PushDataViaPreSignedUrl(fileBytes, JpegMediaType, presignedUrl);
    }
}