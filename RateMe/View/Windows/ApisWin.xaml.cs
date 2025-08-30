using System.Windows;
using RateMe.View.UserControls;

namespace RateMe.View.Windows;

public partial class ApisWin : Window
{
    private const string ApiText =      "Main Api:    {0}";
    private const string S3ServiceText = "S3 Service: {0}";

    public string MainApiAvailability { get; }
    public string S3ServiceAvailability { get; }
    
    public ApisWin(bool isApiAlive, bool isS3ServiceAlive)
    {
        InitializeComponent();
        WindowBarDockPanel bar = new(this);
        WindowGrid.Children.Add(bar);
        bar.expandButton.Visibility = Visibility.Collapsed;
        bar.wrapButton.Visibility = Visibility.Collapsed;

        string apiAv = isApiAlive ? "Available" : "Not available";
        string s3ServAv = isS3ServiceAlive ? "Available" : "Not available";

        MainApiAvailability = string.Format(ApiText, apiAv);
        S3ServiceAvailability = string.Format(S3ServiceText, s3ServAv);
        DataContext = this;
    }

    private void OnOkClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}