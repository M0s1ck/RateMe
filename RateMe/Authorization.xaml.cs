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
        private string _email;
        private string _password;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Welcome back, {_email} : {_password}!");
        }


        private void AuthorizationButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Welcome back!");
            DataCollection dataCollectionWin = new();
            this.Close();
            dataCollectionWin.Show();
        }


        private void emailEntered(object sender, TextChangedEventArgs e)
        {
            if (EmailTextBox.Text == string.Empty)
            {
                EmailHint.Text = "Email";
            }
            else
            {
                EmailHint.Text = string.Empty;
            }

            _email = EmailTextBox.Text;
        }

        private void passwordEntered(object sender, TextChangedEventArgs e)
        {
            if (PasswordTextBox.Text == string.Empty)
            {
                PasswordHint.Text = "Password";
            }
            else
            {
                PasswordHint.Text = string.Empty;
            }

            _password = PasswordTextBox.Text;
        }

    }
}