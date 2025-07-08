using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RateMe.Models.ClientModels;
using RateMe.Services;
using RateMe.Utils.Enums;
using RateMe.Utils.LocalHelpers;

namespace RateMe.ViewModels;

public class ProfileViewModel : INotifyPropertyChanged
{
    private UserService UserService { get; }
    
    public DataHintTextModel NameModel { get; }
    public DataHintTextModel SurnameModel { get; }
    public DataHintTextModel CurriculumModel { get; }
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

    private static readonly Dictionary<string, PictureExtension> ExtentionMap = new()
    { 
        {".png", PictureExtension.Png},
        {".jpg", PictureExtension.Jpg},
        {".jpeg", PictureExtension.Jpeg}
    };

    public ProfileViewModel(UserService userService)
    {
        UserService = userService;
        NameModel = new DataHintTextModel(UserService.User!.Name, "Имя");
        SurnameModel = new DataHintTextModel(UserService.User!.Surname, "Фамилия");
        CurriculumModel = new DataHintTextModel("SE 2028", "оп");
        EmailModel = new DataHintTextModel(UserService.User!.Email, "Email");
        AboutModel = new DataHintTextModel("You know after all that...", "О себе");

        string picturePath = userService.User!.PictureExtension != PictureExtension.None
            ? PictureHelper.CurrentPicturePath
            : PictureHelper.BuildDefaultProfilePicturePath;

        _imageSource = PictureHelper.LoadPicture(picturePath);
    }

    public ProfileViewModel() {}

    public void SaveChanges()
    {
        UserService.User!.Name = NameModel.Data;   // TODO: profile picture + push in db + check for actual changes
        UserService.User!.Surname = SurnameModel.Data;
        UserService.User!.Email = EmailModel.Data;  // Rn just change, maybe after add validations

        if (_pictureChanged)
        {
            string newPicturePath = ((BitmapImage)_imageSource).UriSource.LocalPath;
            PictureHelper.ChangeProfilePicture(newPicturePath);
            
            string ext = Path.GetExtension(newPicturePath);
            UserService.User!.PictureExtension = ExtentionMap[ext];
        }
        
        JsonFileHelper.SaveUser(UserService.User!);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}