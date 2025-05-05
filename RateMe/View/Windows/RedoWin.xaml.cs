using System.Windows;
using System.Windows.Input;
using RateMe.View.UserControls;

namespace RateMe.View.Windows;

public partial class RedoWin : Window
{
    public RedoWin()
    {
        InitializeComponent();
        WindowBarDockPanel bar = new(this);
        WindowGrid.Children.Add(bar);
        bar.expandButton.Visibility = Visibility.Collapsed;
        bar.wrapButton.Visibility = Visibility.Collapsed;
    }
    
    public delegate Task OnRedoAgreed(bool withSave);

    public event OnRedoAgreed? RedoAgreed;
    
    private void YesClick(object sender, RoutedEventArgs e)
    {
        RedoAgreed?.Invoke(true);
        Close();
    }

    private void NoClick(object sender, RoutedEventArgs e)
    {
        RedoAgreed?.Invoke(false);
        Close();
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}