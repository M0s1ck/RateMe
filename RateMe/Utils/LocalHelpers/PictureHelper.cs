using System.IO;
using System.Windows.Media.Imaging;

namespace RateMe.Utils.LocalHelpers;

public static class PictureHelper
{
    public static string BuildDefaultProfilePicturePath { get; } = "pack://application:,,,/Assets/default-profile-picture.jpg";
    
    private static readonly string ProfilePictureName = "profile-picture";
    private static readonly string DataDir = Path.Combine(Directory.GetCurrentDirectory(), "Data");         // Move Up the hierarchy for helpers   
    private static readonly string PicturePathTemplate = Path.Combine(DataDir, ProfilePictureName + "{0}");
    
    public static string CurrentPicturePath { get; private set; } = GetPicturePath();
    
    
    public static void ChangeProfilePicture(string picturePath)
    {
        string ext = Path.GetExtension(picturePath);
        string dataPicturePath = string.Format(PicturePathTemplate, ext);
        
        if (CurrentPicturePath != string.Empty)
        {
            File.Delete(CurrentPicturePath);
        }
        
        File.Copy(picturePath, dataPicturePath);
        CurrentPicturePath = dataPicturePath;
    }

    public static BitmapImage LoadPicture(string path)
    {
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read);
        BitmapImage image = new();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();
        image.Freeze();
        return image;
    }

    private static string GetPicturePath()
    {
        IEnumerable<string> pictureFiles = Directory.EnumerateFiles(DataDir, ProfilePictureName + ".*");
        return pictureFiles.FirstOrDefault() ?? string.Empty;
    }
}