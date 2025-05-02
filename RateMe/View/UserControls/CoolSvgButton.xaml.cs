using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SharpVectors.Converters;

namespace RateMe.View.UserControls;

public partial class CoolSvgButton : UserControl
{
    public static readonly RoutedEvent WhenClickedEvent = EventManager.RegisterRoutedEvent(
        nameof(WhenClicked), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CoolSvgButton));

    public event RoutedEventHandler? WhenClicked; 
    
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(
            nameof(Source), 
            typeof(string), 
            typeof(CoolSvgButton), 
            new PropertyMetadata("/Assets/trash-can.svg")
        );

    public string Source
    {
        get => (string)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    
    public static readonly DependencyProperty SizeProperty = 
        DependencyProperty.Register(
            nameof(Size), 
            typeof(int), 
            typeof(CoolSvgButton), 
            new PropertyMetadata(40)
        );

    public int Size
    {
        get => (int)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
    
    public static readonly DependencyProperty BiggerSizeProperty = 
        DependencyProperty.Register(
            nameof(BiggerSize), 
            typeof(int), 
            typeof(CoolSvgButton), 
            new PropertyMetadata(45)
        );

    public int BiggerSize
    {
        get => (int)GetValue(BiggerSizeProperty);
        set => SetValue(BiggerSizeProperty, value);
    }
    
    public CoolSvgButton()
    {
        InitializeComponent();
    }
    
    private void OnClicked(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(WhenClickedEvent));
    }

    private void OnMouseEnter(object sender, MouseEventArgs e)
    {
        Height = BiggerSize;
        Width = BiggerSize;
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        Height = Size;
        Width = Size;
    }
}