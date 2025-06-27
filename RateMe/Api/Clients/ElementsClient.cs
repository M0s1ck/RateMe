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
    
    public async Task UpdateElems(PlainElem[] dto)
    {
        using HttpResponseMessage response = await TheHttpClient.PutAsJsonAsync("api/subjects/elements", dto);
        string content = await response.Content.ReadAsStringAsync();
        
        // To be continued...
    }
    
    public async Task RemoveElemsByKeys(List<int> keys)
    {
        using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync("api/subjects/elements/delete", keys);
        string content = await response.Content.ReadAsStringAsync();

        // To be continued...
    }
}