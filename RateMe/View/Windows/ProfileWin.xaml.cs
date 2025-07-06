using System.Windows;
using RateMe.Services;
using RateMe.ViewModels;

namespace RateMe.View.Windows;

public partial class ProfileWin : BaseFullWin
{
    public ProfileWin(UserService userService)
    {
        InitializeComponent();
        DataContext = new AccountViewModel(userService);
        Loaded += (_, _) => AddHeaderBar(WindowGrid);
    }

    private void OnEditClick(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Hello");
    }
}