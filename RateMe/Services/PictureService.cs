using System.IO;
using RateMe.Api.Clients;
using RateMe.Utils.LocalHelpers;

namespace RateMe.Services;

public class PictureService
{
    private const string JpegMediaType = "image/jpeg"; 
    
    private PictureClient _pictureClient; 
    
    public PictureService(PictureClient client)
    {
        _pictureClient = client;
    }

    
    public async Task UploadJpgPicture(string path)
    {
        string preSignedUrl = await _pictureClient.GetPreSignedUploadUrl();
        
        byte[] fileBytes = await File.ReadAllBytesAsync(path);

        await _pictureClient.PushDataViaPreSignedUrl(fileBytes, JpegMediaType, preSignedUrl);
    }
    
    
}