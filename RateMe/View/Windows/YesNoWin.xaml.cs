using System.Windows;
using System.Windows.Input;
using RateMe.View.UserControls;

namespace RateMe.View.Windows;

public partial class YesNoWin : Window
{
    public string Question { get; }
    
    public YesNoWin(string question)
    {
        InitializeComponent();
        WindowBarDockPanel bar = new(this);
        WindowGrid.Children.Add(bar);
        bar.expandButton.Visibility = Visibility.Collapsed;
        bar.wrapButton.Visibility = Visibility.Collapsed;

        Question = question;
        DataContext = this;
    }
    
    private void YesClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void NoClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void OnWindowClick(object sender, MouseButtonEventArgs e)
    {
        Keyboard.ClearFocus();
    }
}