using System.Net.Http;
using System.Net.Http.Json;
using RateMeShared.Dto;

namespace RateMe.Api.Clients;

internal class ElementsClient : BaseClient
{
    internal async Task PushElemsBySubsIds(Dictionary<int, List<ControlElementDto>> dto)
    {
        using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync("api/subjects/elements", dto);
        string content = await response.Content.ReadAsStringAsync();
        
        // To be continued...
    }
}