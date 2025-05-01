using RateMe.View.UserControls;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RateMe.DataUtils.JsonModels;
using Path = System.IO.Path;

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
            SetProjectDirectory();
            InitializeComponent();
            
            WindowBarDockPanel bar = new(this);
            WindowGrid.Children.Add(bar);
            
            Config? config = GetConfig();
            OpenNextWin(config);
            
            Close();
        }

        private Config? GetConfig()
        {
            string dir = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            string path = Path.Combine(dir, "config.json");
            string jsonContent = File.ReadAllText(path);

            Config? config = JsonSerializer.Deserialize<Config>(jsonContent);
            
            if (config == null)
            {
                throw new IOException("Couldn't deserialize Data\\config.json");
            }

            return config;
        }

        private void OpenNextWin(Config? config)
        {
            if (config == null || !config.IsSubjectsLoaded)
            {
                DataCollection dataCollectionWin = new DataCollection();
                try
                {
                    dataCollectionWin.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                
            }
            
            
        } 

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Welcome back, {_email} : {_password}!");
        }


        private void AuthorizationButtonClick(object sender, RoutedEventArgs e)
        {
            DataCollection dataCollectionWindow = new();

            try
            {
                dataCollectionWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            this.Close();
            
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

        private static void SetProjectDirectory()
        {
            string defaultPath = Directory.GetCurrentDirectory();
            string[] defaultPathArr = defaultPath.Split(System.IO.Path.DirectorySeparatorChar);
            string projectPath = string.Join(System.IO.Path.DirectorySeparatorChar, defaultPathArr[..^3]);
            Directory.SetCurrentDirectory(projectPath);
        }

    }
}