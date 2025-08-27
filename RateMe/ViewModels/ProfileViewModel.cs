using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RateMe.Models.ClientModels;
using RateMe.Models.JsonFileModels;
using RateMe.Services;
using RateMe.Utils.LocalHelpers;

namespace RateMe.ViewModels;

public class ProfileViewModel : INotifyPropertyChanged
{
    private readonly PictureService _pictureService;
    private readonly UserService _userService;
    private User _user;
    
    public DataHintTextModel NameModel { get; }
    public DataHintTextModel SurnameModel { get; }
    public DataHintTextModel CurriculumModel { get; }
    public DataHintTextModel YearModel { get; }
    public DataHintTextModel EmailModel { get; }
    public DataHintTextModel AboutModel { get; }
    
    private ImageSource _imageSource;
    public ImageSource ImageSource
    {
        get => _imageSource;
        set
        {
            _imageSource = value;
            _pictureChanged = true;
            NotifyPropertyChanged();
        }
    }

    private bool _pictureChanged;
    
    private const int MinYear = 2000;
    private const int MaxYear = 2050;

    
    public ProfileViewModel(UserService userService, PictureService pictureService)
    {
        _pictureService = pictureService;
        _userService = userService;
        _user = userService.User!;
        
        NameModel = new DataHintTextModel(_user.Name, "Имя");
        SurnameModel = new DataHintTextModel(_user.Surname, "Фамилия");
        CurriculumModel = new DataHintTextModel(_user.Curriculum, "оп");
        YearModel = new DataHintTextModel(_user.Year.ToString(), "Год выпуска");
        EmailModel = new DataHintTextModel(_user.Email, "Email");
        AboutModel = new DataHintTextModel(_user.Quote, "О себе");

        _imageSource = _user.IsDefaultPicture ? PictureHelper.LoadDefaultProfilePicture() : PictureHelper.LoadCurrentProfilePicture();
    }
    
    public ProfileViewModel() {}

    
    public async void SaveChanges()
    {
        bool hasUpd = HasDataChanged();

        if (hasUpd)
        {
            UpdateLocal();
        }

        if (_userService.IsRemoteAlive && hasUpd)
        {
            await _userService.UpdateRemoteUser();
        }
        
        if (_pictureChanged)
        {
            UpdateLocalPicture();
        }
        
        if (_pictureChanged && _user.IsDefaultPicture)  // TODO: Add domain healthcheck for S3Service :(, with IsAlive 
        {
            await UploadS3Picture();
        }
        
        if (_pictureChanged && !_user.IsDefaultPicture) 
        {
            await UpdateS3Picture();
        }

        if (_pictureChanged)
        {
            _user.IsDefaultPicture = false;
            JsonFileHelper.SaveUser(_user);
            _pictureChanged = false;
        }
    }
    
    public async Task SignOut()
    {
        await _userService.SignOut();
    }

    private void UpdateLocal()
    {
        _user.Name = NameModel.Data;
        _user.Surname = SurnameModel.Data;
        _user.Email = EmailModel.Data;
        _user.Curriculum = CurriculumModel.Data;
        _user.Quote = AboutModel.Data;

        bool isYearValid = int.TryParse(YearModel.Data, out int year) && year is > MinYear and < MaxYear;

        if (isYearValid)
        {
            _user.Year = year;
        }
        else
        {
            YearModel.Data = _user.Year.ToString();
        }

        _userService.UpdateUser(_user);
    }

    private void UpdateLocalPicture()
    {
        BitmapImage newPicture = (BitmapImage)_imageSource;
        PictureHelper.ChangeProfilePicture(newPicture);
    }

    private async Task UploadS3Picture()  // TODO: Add to back s3Ids
    {
        string? picId = await _pictureService.UploadJpgPicture(PictureHelper.ProfilePicturePathJpg);

        if (picId != null)
        {
            _user.PictureS3Id = picId;
            JsonFileHelper.SaveUser(_user);
        }
    }

    private async Task UpdateS3Picture()
    {
        string id = _user.PictureS3Id;
        await _pictureService.UpdateJpgPicture(id, PictureHelper.ProfilePicturePathJpg);
    }

    private bool HasDataChanged()
    {
        return _user.Name != NameModel.Data || _user.Surname != SurnameModel.Data || _user.Email != EmailModel.Data ||
               _user.Curriculum != CurriculumModel.Data || _user.Quote != AboutModel.Data;
    }

    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}