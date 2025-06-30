using RateMe.Api.Clients;
using RateMeShared.Dto;
using System.Net.Http;
using System.Net.Sockets;
using System.Windows;
using RateMe.Models.JsonModels;

namespace RateMe.Services;

public class UserService
{
    private readonly UserClient _userClient;

    internal UserService()
    {
        _userClient = new UserClient();
    }
    
    
    internal async Task SignUp(string email, string pass, string name, string surname)
    {
        await SignUpSignInStart?.Invoke()!;
        
        UserDto userDto = new() { Email = email, Password = pass, Name = name, Surname = surname };
        int id = 0;
        
        try
        {
            id = await _userClient.SignUpUserAsync(userDto);
        }
        catch (HttpRequestException ex)
        {
            HandleHttpException(ex);
        }

        if (id == 0)
        {
            return;
        }
        
        User user = new(userDto);
        user.Id = id;
        JsonModelsHandler.SaveUser(user);
        
        await SignUpSuccess?.Invoke()!;
        MessageBox.Show($"You've been signed up! Your id: {id}");
    }
    
    
    public delegate Task AsyncHandler();
    
    public event AsyncHandler? SignUpSignInStart;
    
    public event AsyncHandler? SignUpSuccess;
    
    public event AsyncHandler? SignInSuccess;
    

    internal async Task SignIn(string email, string pass)
    {
        await SignUpSignInStart?.Invoke()!;
        
        AuthRequest request = new() { Email = email, Password = pass };
        UserDto? userDto = null; 
        
        try
        {
            userDto = await _userClient.AuthUserAsync(request);
        }
        catch (HttpRequestException ex)
        {
            HandleHttpException(ex);
        }

        if (userDto == null)
        {
            return;
        }
        
        User user = new User(userDto);
        MessageBox.Show($"Hello, {user.Name} {user.Surname}");
        JsonModelsHandler.SaveUser(user);
    }
    
    
    private void HandleHttpException(HttpRequestException ex)
    {
        Type? exType = ex.InnerException?.GetType();
        string msg = exType == typeof(SocketException) ? "Похоже сервер не отвечает(" : ex.ToString();
        MessageBox.Show(msg); //TODO: make more appealing?
    }
}