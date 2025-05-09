using System.Net.Http;
using RateMe.Models.JsonModels;
using RateMe.Models.DtoModels;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Windows;

namespace RateMe.Api
{
    class ServerApi
    {
        private readonly HttpClient _httpClient;

        public ServerApi()
        {
            Config config = JsonModelsHandler.GetConfig();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(config.ApiUrl);
        }

        public async Task PostUserAsync(User user)
        {
            try
            {
                using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("User", user);
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException?.GetType() ==  typeof(SocketException))
                {
                    MessageBox.Show("Похоже сервер не отвечает(");
                }
                else
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        
    }
}
