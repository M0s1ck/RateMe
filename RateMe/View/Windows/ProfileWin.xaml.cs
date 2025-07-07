using System.Windows;
using RateMe.Services;
using RateMe.ViewModels;

namespace RateMe.View.Windows;

public partial class ProfileWin : BaseFullWin
{
    private ProfileViewModel _viewModel;
    
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
    }

    private void OnEditClick(object sender, RoutedEventArgs e)
    {
        SwitchVisibility(EditInfo);
        SwitchVisibility(NormalInfo);
        SwitchVisibility(SaveButton);
    }

    private void SwitchVisibility(UIElement uiElement)
    {
        uiElement.Visibility = uiElement.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
    }
}