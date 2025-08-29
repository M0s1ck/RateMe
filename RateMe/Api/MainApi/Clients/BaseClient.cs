using System.Net.Http;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RateMe.Utils.LocalHelpers;

namespace RateMe.Api.MainApi.Clients;

public class BaseClient
{
    protected HttpClient TheHttpClient { get; init; } = new HttpClient() { Timeout = TimeSpan.FromSeconds(20) };  // TODO: decrease

    protected static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected static readonly Uri BaseUri = new Uri(JsonFileHelper.GetConfig().ApiUrl);

    public async Task<bool> IsRemoteAlive()
    {
        Console.WriteLine($"Testing connection to main api at {BaseUri}");
        
        try
        {
            HttpResponseMessage response = await TheHttpClient.GetAsync(BaseUri);
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Main api connection successful");
            }
            else
            {
                Console.WriteLine($"Main api connection issue: {response.StatusCode}");
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            Console.WriteLine("Main api is unavailable :(");
            return false;
        }
    } 
}