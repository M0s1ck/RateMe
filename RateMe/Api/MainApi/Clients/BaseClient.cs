using System.Net.Http;
using System.Text.Json;
using RateMe.Utils.LocalHelpers;

namespace RateMe.Api.MainApi.Clients;

public class BaseClient
{
    protected HttpClient TheHttpClient { get; init; } = new HttpClient() { Timeout = TimeSpan.FromSeconds(2) };

    protected static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected static readonly Uri BaseUri = new Uri(JsonFileHelper.GetConfig().ApiUrl);

    public async Task<bool> IsRemoteAlive()
    {
        try
        {
            HttpResponseMessage response = await TheHttpClient.GetAsync(BaseUri);
            return response.IsSuccessStatusCode;
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return false;
        }
    } 
}