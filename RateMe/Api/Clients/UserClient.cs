﻿using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using System.Windows;
using RateMeShared.Dto;

namespace RateMe.Api.Clients
{
    internal class UserClient : BaseClient
    {
        public UserClient() : base() { }
        
        
        public async Task<int?> SignUpUserAsync(UserDto userDto)
        {
            try
            {
                using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync("api/users/signup", userDto);
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
                    MessageBox.Show($"Unhandled response: {response.StatusCode} {msg} ");
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

        
        public async Task<UserDto?> AuthUserAsync(AuthRequest authRequest) //TODO: wrap in try catch
        {
            using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync("api/users/auth", authRequest);
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
                UserDto? user = JsonSerializer.Deserialize<UserDto>(msg, options: CaseInsensitiveOptions);
                return user;
            }
            else
            {
                MessageBox.Show($"Unhandled response: {msg}");
            }

            return null;
        }
        
    }
}
