using RateMe.Models.ClientModels;
using System.Windows;
using System.Windows.Controls;

namespace RateMe.View.UserControls;

/// <summary>
/// Логика взаимодействия для DataHintTextBox.xaml
/// </summary>
public partial class DataHintTextBox : UserControl
{
    public static readonly DependencyProperty TextBoxFontSizeProp =
        DependencyProperty.Register("TextBoxFontSize", typeof(double), typeof(DataHintTextBox), new PropertyMetadata(15.0));

    public static readonly DependencyProperty TextBoxHeightProp =
        DependencyProperty.Register("TextBoxHeight", typeof(double), typeof(DataHintTextBox), new PropertyMetadata(30.0));

    public static readonly DependencyProperty TextBoxMinWidthProp =
        DependencyProperty.Register("TextBoxMinWidth", typeof(double), typeof(DataHintTextBox), new PropertyMetadata(120.0));

    public static readonly DependencyProperty TextBoxMaxWidthProp =
        DependencyProperty.Register("TextBoxMaxWidth", typeof(double), typeof(DataHintTextBox), new PropertyMetadata(1000.0));

    public static readonly DependencyProperty TextBlockFontSizeProp =
        DependencyProperty.Register("TextBlockFontSize", typeof(double), typeof(DataHintTextBox), new PropertyMetadata(13.0));

    public static readonly DependencyProperty TextBlockWidthProp =
        DependencyProperty.Register("TextBlockWidth", typeof(double), typeof(DataHintTextBox), new PropertyMetadata(150.0));


    public double TextBoxFontSize
    {
        get => (double)GetValue(TextBoxFontSizeProp);
        set => SetValue(TextBoxFontSizeProp, value);
    }

    public double TextBoxHeight
    {
        get => (double)GetValue(TextBoxHeightProp);
        set => SetValue(TextBoxHeightProp, value);
    }

    public double TextBoxMinWidth
    {
        get => (double)GetValue(TextBoxMinWidthProp);
        set => SetValue(TextBoxMinWidthProp, value);
    }

    public double TextBoxMaxWidth
    {
        get => (double)GetValue(TextBoxMaxWidthProp);
        set => SetValue(TextBoxMaxWidthProp, value);
    }

    public double TextBlockFontSize
    {
        get => (double)GetValue(TextBlockFontSizeProp);
        set => SetValue(TextBlockFontSizeProp, value);
    }
    public double TextBlockWidth
    {
        get => (double)GetValue(TextBlockWidthProp);
        set => SetValue(TextBlockWidthProp, value);
    }


    public DataHintTextBox()
    {
        InitializeComponent();
    }

    private void OnUpdatedText(object sender, TextChangedEventArgs e)
    {
        ((DataHintTextModel)DataContext).Data = dataTextBox.Text;    // NUll ref
    }
}