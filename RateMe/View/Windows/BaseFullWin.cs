using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RateMe.View.UserControls;

namespace RateMe.View.Windows;

public abstract class BaseFullWin : Window
{
    protected void AddHeaderBar(Grid windowGrid)
    {
        WindowBarDockPanel bar = new(this);
        windowGrid.Children.Add(bar);
    }
    
    protected void OnWindowClick(object sender, MouseButtonEventArgs e)
    {
        Keyboard.ClearFocus();
    }
}