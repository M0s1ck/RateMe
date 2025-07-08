using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using RateMe.Services;
using RateMe.ViewModels;

namespace RateMe.View.Windows;

public partial class ProfileWin : BaseFullWin
{
    private readonly ProfileViewModel _viewModel;
    
    public ProfileWin(UserService userService)
    {
        InitializeComponent();
        _viewModel = new ProfileViewModel(userService);  
        DataContext = _viewModel;
        Loaded += (_, _) => AddHeaderBar(WindowGrid);
    }
    
    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        _viewModel.SaveChanges();
        
        SwitchVisibility(EditInfo);
        SwitchVisibility(NormalInfo);
        SwitchVisibility(SaveButton);
        SwitchVisibility(EditPictureButton);
    }

    private void OnEditClick(object sender, RoutedEventArgs e)
    {
        SwitchVisibility(EditInfo);
        SwitchVisibility(NormalInfo);
        SwitchVisibility(SaveButton);
        SwitchVisibility(EditPictureButton);
    }

    private void SwitchVisibility(UIElement uiElement)
    {
        uiElement.Visibility = uiElement.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OnEditPictureClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog op = new OpenFileDialog();
        op.Title = "Select a picture (preferably a square one)";
        op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                    "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                    "Portable Network Graphic (*.png)|*.png";
        
        if (op.ShowDialog() == true)
        {
            _viewModel.ImageSource = new BitmapImage(new Uri(op.FileName));
        }
    }
}