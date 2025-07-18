using System.Net;
using System.Net.Http;
using System.Text.Json;
using RateMe.Utils.LocalHelpers;

namespace RateMe.Api.MinIoApi;

public class PictureClient
{
    private const string UrlTemplate = "pictures/{0}/";
    private readonly HttpClient _httpClient; 
    
    public PictureClient(int userId)
    {
        _httpClient = new HttpClient();
        Uri domen = new Uri(JsonFileHelper.GetConfig().S3Url);
        string relativePath = string.Format(UrlTemplate, userId);
        _httpClient.BaseAddress = new Uri(domen, relativePath);
    }

    public async Task<string> GetPreSignedUploadUrl()
    {
        using HttpResponseMessage response = await _httpClient.GetAsync("upload");

        if (response.StatusCode != HttpStatusCode.Created)
        {
            return string.Empty;
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
        
    }
}