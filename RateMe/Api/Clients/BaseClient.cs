using System.Net.Http;
using System.Text.Json;
using RateMe.Models.JsonFileModels;

namespace RateMe.Api.Clients;

public abstract class BaseClient
{
    protected HttpClient TheHttpClient { get; init; } = null!;

    protected static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected static Uri BaseUri = new Uri(JsonFileModelsHelper.GetConfig().ApiUrl);
}