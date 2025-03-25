using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RateMe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (WelcomeBackButton.IsChecked != null && WelcomeBackButton.IsChecked == true)
            {
                MessageBox.Show("Welcome back!");
            }
            else if (WelcomeButton.IsChecked != null && WelcomeButton.IsChecked == true)
            {
                MessageBox.Show("Welcome!");
            }
        }
    }
}