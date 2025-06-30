using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using RateMeShared.Dto;

namespace RateMe.Api.Clients;

internal class ElementsClient : BaseClient
{
    internal ElementsClient(int userId)
    {
        TheHttpClient = new HttpClient();
        string relativePath = $"api/users/{userId}/subjects/elements/";
        TheHttpClient.BaseAddress = new Uri(BaseUri, relativePath);
    }
    
    internal async Task<Dictionary<int, int>?> PushElemsBySubsIds(Dictionary<int, List<ElementDto>> dto)
    {
        using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync(string.Empty, dto);
        string content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            Dictionary<int, int> localRemoteKeys = JsonSerializer.Deserialize<Dictionary<int, int>>(content, options: CaseInsensitiveOptions)!;
            return localRemoteKeys;
        }

        MessageBox.Show($"Elems Push fail:{response.StatusCode}:{content}");
        return null;
    }
    
    public async Task UpdateElems(PlainElem[] dto)
    {
        using HttpResponseMessage response = await TheHttpClient.PutAsJsonAsync(string.Empty, dto);
        string content = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode != HttpStatusCode.NoContent)
        {
            MessageBox.Show($"{response.StatusCode}:{content}");
        }
    }
    
    public async Task RemoveElemsByKeys(HashSet<int> keys)
    {
        using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync("delete", keys);
        string content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode != HttpStatusCode.NoContent)
        {
            MessageBox.Show($"{response.StatusCode}:{content}");
        }
    }
}