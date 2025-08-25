using System.IO;
using RateMe.Api.S3ServiceApi;
using RateMeShared.Dto;

namespace RateMe.Services;

public class PictureService
{
    private const string JpegMediaType = "image/jpeg"; 
    
    private PictureClient _pictureClient;
    
    public PictureService(PictureClient client)
    {
        _pictureClient = client;
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
    
    
}