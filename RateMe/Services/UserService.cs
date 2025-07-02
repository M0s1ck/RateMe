using RateMe.Api.Clients;
using RateMeShared.Dto;
using System.Net.Http;
using System.Net.Sockets;
using System.Windows;
using RateMe.Models.JsonModels;
using RateMe.Services.Interfaces;

namespace RateMe.Services;

public class UserService
{
    internal bool IsUserAvailable => _user != null;
    
    private ISubjectUpdater _subjectService;
    private IElemUpdater _elemService;
    
    private readonly UserClient _userClient;
    private User? _user;
    
    
    internal UserService(ISubjectUpdater subjService, IElemUpdater elemService)
    {
        _subjectService = subjService;
        _elemService = elemService;
        
        _userClient = new UserClient();
        _user = JsonModelsHandler.GetUserOrNull();

        if (_user != null)
        {
            _subjectService.UpdateUserId(_user.Id);
            _elemService.UpdateUserId(_user.Id);
        }
    }
    
    
    internal async Task SignUp(string email, string pass, string name, string surname)
    {
        if (IsUserAvailable)
        {
            MessageBox.Show("You are already signed in!");
            return;
        }

        await _subjectService.UpdateAllLocals();
        
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
        
        _user = new User(userDto);
        _user.Id = id;
        UpdateOnUser();

        await _subjectService.SubjectsOverallRemoteUpdate();
        // Пока что если sign up можно сделать только один раз то не нужен _elementsService.ElementsOverallRemoteUpdate()
        MessageBox.Show($"You've been signed up! Your id: {id}");
    }
    

    internal async Task SignIn(string email, string pass)
    {
        if (!IsUserAvailable)
        {
            MessageBox.Show("You are not signed up! All local data will be lost. You better sign up first");
            return; // TODO: what if still sign in? Add yes/no window, disable others wins to answer rn
        }
        else
        {
            _subjectService.RetainSubjectsToUpdate();
            _elemService.RetainElemsToUpdate();
            
            await _subjectService.UpdateAllLocals();
            
            await _subjectService.SubjectsOverallRemoteUpdate();
            await _elemService.ElementsOverallRemoteUpdate();
        }
        
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
        
        _user = new User(userDto);
        UpdateOnUser();
        MessageBox.Show($"Hello, {_user.Name} {_user.Surname}");

        await _subjectService.LoadAllUserSubjectsFromRemote();
    }

    private void UpdateOnUser()
    {
        JsonModelsHandler.SaveUser(_user!);
        _subjectService.UpdateUserId(_user!.Id);
        _elemService.UpdateUserId(_user!.Id);
    }
    
    private void HandleHttpException(HttpRequestException ex)
    {
        Type? exType = ex.InnerException?.GetType();
        string msg = exType == typeof(SocketException) ? "Похоже сервер не отвечает(" : ex.ToString();
        MessageBox.Show(msg); //TODO: make more appealing?
    }
}