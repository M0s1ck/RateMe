using System.Net.Http;
using System.Text.Json;
using RateMe.Models.JsonModels;

namespace RateMe.Api.Clients;

public abstract class BaseClient
{
    protected readonly HttpClient TheHttpClient;
    
    protected static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    protected BaseClient()
    {
        Config config = JsonModelsHandler.GetConfig();
        TheHttpClient = new HttpClient();
        TheHttpClient.BaseAddress = new Uri(config.ApiUrl);
    }
}