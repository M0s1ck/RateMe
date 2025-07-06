using RateMe.Api.Clients;
using RateMeShared.Dto;
using System.Net.Http;
using System.Net.Sockets;
using System.Windows;
using RateMe.Models.JsonFileModels;
using RateMe.Services.Interfaces;
using RateMe.View.Windows;

namespace RateMe.Services;

public class UserService
{
    public User? User { get; private set; }
    internal bool IsUserAvailable => User != null;
    
    private ISubjectUpdater _subjectService;
    private IElemUpdater _elemService;
    
    private readonly UserClient _userClient;
    
    
    
    internal UserService(ISubjectUpdater subjService, IElemUpdater elemService)
    {
        _subjectService = subjService;
        _elemService = elemService;
        
        _userClient = new UserClient();
        User = JsonFileModelsHelper.GetUserOrNull();

        if (User != null)
        {
            _subjectService.SubjClient = new SubjectsClient(User.Id);
            _elemService.ElemClient = new ElementsClient(User.Id);
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
        
        User = new User(userDto);
        User.Id = id;
        UpdateOnUser();

        await _subjectService.SubjectsOverallRemoteUpdate();
        // In theory не нужен _elementsService.ElementsOverallRemoteUpdate()
        MessageBox.Show($"You've been signed up! Your id: {id}");
    }
    

    internal async Task SignIn(string email, string pass, bool safe=true)
    {
        if (!IsUserAvailable && safe && _subjectService.IsAnyData)
        {
            WannaSignInNoSignUp(email, pass);
            return; // TODO: what if still sign in? Add yes/no window, disable others wins to answer rn
        }           // If still yes, we just remove all locals and continue
                    // How to make so that we really sign up after "yes" - maybe just add safe bool flag param that's by
                    // default is true and call func 

        try
        {
            await OverallSaveUpdate();
        }
        catch (HttpRequestException ex)
        {
            HandleHttpException(ex);
            await _subjectService.MarkRemoteStates();
            await _elemService.MarkRemoteStates();
            return;
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
        
        User = new User(userDto);
        UpdateOnUser();
        MessageBox.Show($"Hello, {User.Name} {User.Surname}");

        await _subjectService.LoadUpdateAllUserSubjectsFromRemote();
    }


    // Should be available only if userIsAvailable
    internal async Task SignOut()
    {
        if (!IsUserAvailable)
        {
            return;
        }

        try
        {
            await OverallSaveUpdate();
        }
        catch (HttpRequestException ex)
        {
            HandleHttpException(ex);
            return;
        }
        
        JsonFileModelsHelper.RemoveUser();
        User = null;

        await _subjectService.ClearLocal();
    }


    private async Task OverallSaveUpdate()
    {
        _subjectService.RetainSubjectsToUpdate();
        _elemService.RetainElemsToUpdate();
            
        await _subjectService.UpdateAllLocals();
            
        await _subjectService.SubjectsOverallRemoteUpdate();
        await _elemService.ElementsOverallRemoteUpdate();
    }
    
    
    private void UpdateOnUser()
    {
        JsonFileModelsHelper.SaveUser(User!);
        _subjectService.SubjClient = new SubjectsClient(User!.Id);
        _elemService.ElemClient = new ElementsClient(User.Id);
    }


    private void WannaSignInNoSignUp(string email, string pass)
    {
        string question = "You are not signed up! All local data will be lost. Continue?";
        YesNoWin win = new(question);
        
        win.YesButton.Click += async (o, args) =>
        {
            await _subjectService.ClearLocal();
            await SignIn(email, pass, safe: false); 
        };
        
        win.Show();
    }
    
    private void HandleHttpException(HttpRequestException ex)
    {
        Type? exType = ex.InnerException?.GetType();
        string msg = exType == typeof(SocketException) ? "Похоже сервер не отвечает(" : ex.ToString();
        MessageBox.Show(msg); //TODO: make more appealing?
    }
}