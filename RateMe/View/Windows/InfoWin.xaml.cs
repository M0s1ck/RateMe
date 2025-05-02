using System.Windows;
using RateMe.View.UserControls;

namespace RateMe.View.Windows;

public partial class InfoWin : Window
{
    public InfoWin()
    {
        InitializeComponent();
        WindowBarDockPanel bar = new(this);
        WindowGrid.Children.Add(bar);
        bar.expandButton.Visibility = Visibility.Collapsed;
        bar.wrapButton.Visibility = Visibility.Collapsed;
    }

    private void OnOkClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}