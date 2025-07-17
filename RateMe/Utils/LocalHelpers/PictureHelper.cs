using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RateMe.Utils.LocalHelpers;

public static class PictureHelper
{
    public const string BuildDefaultProfilePicturePath = "pack://application:,,,/Assets/default-profile-picture.jpg";
    
    private static readonly string DataDir = Path.Combine(Directory.GetCurrentDirectory(), "Data");         // Move Up the hierarchy for helpers
    
    public static readonly string ProfilePicturePathJpg = Path.Combine(DataDir, "profile-picture.jpg");
    
    
    public static void ChangeProfilePicture(BitmapImage image)
    {
        CroppedBitmap croppedToSquare = CropToSquare(image);
        
        using FileStream stream = new(ProfilePicturePathJpg, FileMode.Create, FileAccess.Write);
        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(croppedToSquare));
        encoder.Save(stream);
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

    private static CroppedBitmap CropToSquare(BitmapImage source)
    {
        int width = source.PixelWidth;
        int height = source.PixelHeight;
        
        int side = Math.Min(width, height);
        
        int x = (width - side) / 2;
        int y = (height - side) / 2;
        
        CroppedBitmap cropped = new CroppedBitmap(source, new Int32Rect(x, y, side, side));

        return cropped;
    } 
}