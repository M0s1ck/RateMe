using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using RateMe.Utils.LocalHelpers;
using RateMeShared.Dto;

namespace RateMe.Api.S3ServiceApi;

public class PictureClient
{
    private const string GetPath = "/presigned/{0}";
    private const string UploadPath = "/presigned/upload";
    private const string UpdatePath = "/presigned/upload/{0}";

    private static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    private readonly HttpClient _httpClient; 
    
    public PictureClient()
    {
        _httpClient = new HttpClient();
        Uri domain  = new Uri(JsonFileHelper.GetConfig().S3Url);
        _httpClient.BaseAddress = domain;
    }

    public async Task<string> GetPresignedGetUrl(string id)
    {
        string rPath = string.Format(GetPath, id);
        using HttpResponseMessage response = await _httpClient.GetAsync(rPath);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            MessageBox.Show($"S3 service error: {response.StatusCode}");
            return string.Empty;
        }
        
        using JsonDocument json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        string url = json.RootElement.GetProperty("url").ToString();
        return url;
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
    
    public async Task<string?> GetPreSignedUpdateUrl(string id)
    {
        string path = string.Format(UpdatePath, id);
        using HttpResponseMessage response = await _httpClient.PutAsync(path, null);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            MessageBox.Show($"S3 service error: {response.StatusCode}");
            return null;
        }
        
        using JsonDocument json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        string url = json.RootElement.GetProperty("url").ToString();
        return url;
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

    public async Task<MemoryStream?> GetDataViaPresignedUrl(string url)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(url);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            MessageBox.Show($"Minio error: {response.StatusCode}");
            return null;
        }

        await using Stream stream = await response.Content.ReadAsStreamAsync();
        
        MemoryStream memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        memory.Position = 0;
        return memory;
    } 
}