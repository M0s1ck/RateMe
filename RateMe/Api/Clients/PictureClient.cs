using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace RateMe.Api.Clients;

public class PictureClient : BaseClient
{
    private const string UrlTemplate = "api/users/{0}/picture/";
    private HttpClient _plainHttpClient; 
    
    public PictureClient(int userId)
    {
        TheHttpClient = new HttpClient();
        string relativePath = string.Format(UrlTemplate, userId);
        TheHttpClient.BaseAddress = new Uri(BaseUri, relativePath);
        _plainHttpClient = new HttpClient();
    }

    public async Task<string> GetPreSignedUploadUrl()
    {
        using HttpResponseMessage response = await TheHttpClient.GetAsync("upload");

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
        
        using HttpResponseMessage response = await _plainHttpClient.PutAsync(presignedUrl, content);
        
    }
}