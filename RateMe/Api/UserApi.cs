using System.Net;
using System.Net.Http;
using RateMe.Models.JsonModels;
using RateMe.Models.DtoModels;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using System.Windows;

namespace RateMe.Api
{
    internal class UserApi
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        
        public UserApi()
        {
            Config config = JsonModelsHandler.GetConfig();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(config.ApiUrl);
        }

        public async Task<int?> SignUpUserAsync(User user)
        {
            try
            {
                using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/User/signup", user);
                string msg = await response.Content.ReadAsStringAsync();
                
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    MessageBox.Show(msg); //email is taken TODO: make more appealing?
                }
                else if (response.StatusCode == HttpStatusCode.OK)
                {
                    int id = int.Parse(msg);
                    return id;
                }
                else
                {
                    MessageBox.Show($"Unhandled response:  {msg}");
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException?.GetType() ==  typeof(SocketException))
                {
                    MessageBox.Show("Похоже сервер не отвечает("); //TODO: make more appealing?
                }
                else
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            return null;
        }

        public async Task<User?> AuthUserAsync(AuthRequest authRequest)
        {
            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/User/auth", authRequest);
            string msg = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                MessageBox.Show(msg); // неверный пароль
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                MessageBox.Show(msg); // нет такой почты
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                User? user = JsonSerializer.Deserialize<User>(msg, options: JsonOptions);
                return user;
            }
            else
            {
                MessageBox.Show($"Unhandled response:  {msg}");
            }

            return null;
        }
        
    }
}
