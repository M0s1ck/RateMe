using RateMeShared.Dto;
using System.Windows;
using RateMe.Api.MainApi.Clients;
using RateMe.Api.MainApi.Mappers;
using RateMe.Models.JsonFileModels;
using RateMe.Services.Interfaces;
using RateMe.Utils.LocalHelpers;
using RateMe.View.Windows;

namespace RateMe.Services;

public class UserService
{
    public User? User { get; private set; }
    internal bool IsUserAvailable => User != null;
    public bool IsRemoteAlive { get; }
    
    private ISubjectUpdater _subjectService;
    private IElemUpdater _elemService;
    private PictureService _picService;
    
    private readonly UserClient _userClient;
    
    public event Action? SignedOut;
    
    
    internal UserService(ISubjectUpdater subjService, IElemUpdater elemService, PictureService picService, bool isRemoteAlive)
    {
        _subjectService = subjService;
        _elemService = elemService;
        _picService = picService;
        
        _userClient = new UserClient();
        User = JsonFileHelper.GetUserOrNull();
        IsRemoteAlive = isRemoteAlive;

        if (User != null)
        {
            _subjectService.SubjClient = new SubjectsClient(User.Id);
            _elemService.ElemClient = new ElementsClient(User.Id);
        }
    }
    
    
    internal async Task SignUp(string email, string pass, string name, string surname)
    {
        if (!IsRemoteAlive)
        {
            MessageBox.Show("К сожалению сервер сейчас не доступен(");
            return;
        }
        
        if (IsUserAvailable)
        {
            MessageBox.Show("You are already signed in!");
            return;
        }

        await _subjectService.UpdateAllLocals();
        
        UserDto userDto = new() { Email = email, Password = pass, Name = name, Surname = surname };
        int id = await _userClient.SignUpUserAsync(userDto);

        if (id == 0)
        {
            return;
        }
        
        User = new User(userDto);
        User.Id = id;
        UpdateOnUser();

        await _subjectService.SubjectsOverallRemoteUpdate();
        MessageBox.Show($"You've been signed up!");
    }
    

    internal async Task SignIn(string email, string pass, bool safe=true)
    {
        if (!IsRemoteAlive)
        {
            MessageBox.Show("К сожалению сервер сейчас не доступен(");
            await _subjectService.MarkRemoteStates();
            await _elemService.MarkRemoteStates();
            return;
        }
        
        if (!IsUserAvailable && safe && _subjectService.IsAnyData)
        {
            WannaSignInNoSignUp(email, pass);
            return;
        }           
        
        await OverallSaveUpdate();
        
        AuthRequest request = new() { Email = email, Password = pass };
        UserFullDto? userDto = await _userClient.AuthUserAsync(request);

        if (userDto == null)
        {
            return;
        }

        User = UserMapper.UserFromFullDto(userDto);
        UpdateOnUser();

        await _subjectService.LoadUpdateAllUserSubjectsFromRemote();

        if (User.PictureS3Id != null && _picService.IsServiceAlive)  
        {
            await _picService.LoadPictureFromS3(User.PictureS3Id);
            User.IsDefaultPicture = false;
            JsonFileHelper.SaveUser(User);
        }
    }

    
    internal void UpdateUser(User user)
    {
        User = user;
        JsonFileHelper.SaveUser(user);
    }

    internal async Task UpdateRemoteUser()
    {
        UserFullDto fullDto = UserMapper.UserToFullDto(User!);
        await _userClient.UpdateUser(fullDto);
        User!.IsRemoteUpdated = true;
    }

    internal async Task UpdateS3PicId(string s3Id)
    {
        int userId = User!.Id;
        await _userClient.UpdateS3PicId(userId, s3Id);
    } 
    
    internal async Task SignOut()
    {
        if (!IsUserAvailable)
        {
            return;
        }

        if (IsRemoteAlive)
        {
            await OverallSaveUpdate();
        }
        
        JsonFileHelper.RemoveUser();
        User = null;
        
        PictureHelper.RemoveProfilePicture();

        await _subjectService.ClearLocal();
        SignedOut?.Invoke();
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
        JsonFileHelper.SaveUser(User!);
        _subjectService.SubjClient = new SubjectsClient(User!.Id);
        _elemService.ElemClient = new ElementsClient(User.Id);
    }

    private void WannaSignInNoSignUp(string email, string pass)
    {
        string question = "You are not signed up! All local data will be lost. Continue?";
        YesNoWin win = new(question);
        
        win.YesButton.Click += async (_, _) =>
        {
            await _subjectService.ClearLocal();
            await SignIn(email, pass, safe: false); 
        };
        
        win.Show();
    }
}