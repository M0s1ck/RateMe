using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using RateMe.Utils.LocalHelpers;
using RateMeShared.Dto;

namespace RateMe.Api.S3ServiceApi;

public class PictureClient
{
    private const string UploadPath = "/presigned/upload";

    private static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    private readonly HttpClient _httpClient; 
    
    public PictureClient(int userId)
    {
        _httpClient = new HttpClient();
        Uri domain  = new Uri(JsonFileHelper.GetConfig().S3Url);
        _httpClient.BaseAddress = domain;
    }

    public async Task<PresignedUploadDto?> GetPreSignedUploadUrl()
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(UploadPath);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            MessageBox.Show($"S3 service error: {response.StatusCode}");
            return null;
        }
        
        string msg = await response.Content.ReadAsStringAsync();
        PresignedUploadDto? uploadDto = JsonSerializer.Deserialize<PresignedUploadDto>(msg, options: CaseInsensitiveOptions);
        return uploadDto;
    }

    public async Task PushDataViaPreSignedUrl(byte[] dataBytes, string mediaType, string presignedUrl)
    {
        using ByteArrayContent content = new ByteArrayContent(dataBytes);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);
        
        using HttpResponseMessage response = await _httpClient.PutAsync(presignedUrl, content);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            MessageBox.Show($"Minio error: {response.StatusCode}");
        }
    }
}