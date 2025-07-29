using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using RateMeShared.Dto;

namespace RateMe.Api.MainApi.Clients;

internal class UserClient : BaseClient
{
    private const string RelativePath = "api/users/";
    
    public UserClient()
    {
        TheHttpClient = new HttpClient();
        TheHttpClient.BaseAddress = new Uri(BaseUri, RelativePath);
    }
        
        
    public async Task<int> SignUpUserAsync(UserDto userDto)
    {
        using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync("signup", userDto);
            
        if (response.StatusCode == HttpStatusCode.Created)
        {
            using JsonDocument json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            int userId = json.RootElement.GetProperty("id").GetInt32();
            return userId;
        }
            
        string msg = await response.Content.ReadAsStringAsync();
            
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            MessageBox.Show($"Неверные данные: {msg}");
        }
            
        MessageBox.Show($"Unhandled response: {response.StatusCode} {msg} ");
        return 0;
    }


    public async Task<UserFullDto?> AuthUserAsync(AuthRequest authRequest)
    {
        using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync("auth", authRequest);
        string msg = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            UserFullDto? user = JsonSerializer.Deserialize<UserFullDto>(msg, options: CaseInsensitiveOptions);
            return user;
        }

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.NotFound)
        {
            MessageBox.Show($"Неверные данные: {msg}");
            return null;
        }

        MessageBox.Show($"Unhandled response: {msg}");
        return null;
    }

    public async Task UpdateUser(UserFullDto userFullDto)
    {
        using HttpResponseMessage response = await TheHttpClient.PatchAsJsonAsync(userFullDto.Id.ToString(), userFullDto);
    }
}