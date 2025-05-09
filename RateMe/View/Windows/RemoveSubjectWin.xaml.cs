using System.Windows;
using System.Windows.Input;
using RateMe.Models.ClientModels;
using RateMe.View.UserControls;

namespace RateMe.View.Windows;

public partial class RemoveSubjectWin : Window
{
    private Subject _subject;
    
    public RemoveSubjectWin(Subject subject)
    {
        InitializeComponent();
        WindowBarDockPanel bar = new(this);
        WindowGrid.Children.Add(bar);
        bar.expandButton.Visibility = Visibility.Collapsed;
        bar.wrapButton.Visibility = Visibility.Collapsed;

        _subject = subject;
        DataContext = subject;
    }
    
    public delegate void OnRemovalAgreed(Subject subject);

    public event OnRemovalAgreed? RemovalAgreed;
    
    private void YesClick(object sender, RoutedEventArgs e)
    {
        RemovalAgreed?.Invoke(_subject);
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