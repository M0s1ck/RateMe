using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RateMe.View.UserControls;

public partial class PlusButton : UserControl
{
    static readonly SolidColorBrush ColorWhenEntered = new SolidColorBrush(Colors.DimGray);
    static readonly SolidColorBrush ColorWhenLeft = new SolidColorBrush(Colors.White);
    
    public PlusButton()
    {
        InitializeComponent();
    }
    
    private void OnAddMouseEnter(object sender, MouseEventArgs e)
    {
        vertBar.Fill = ColorWhenEntered;
        horBar.Fill = ColorWhenEntered;
    }

    private void OnAddMouseLeave(object sender, MouseEventArgs e)
    {
        vertBar.Fill = ColorWhenLeft;
        horBar.Fill = ColorWhenLeft;
    }
}