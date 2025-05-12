using System.Windows;
using System.Windows.Controls;
using RateMe.Models.ClientModels;
using RateMe.Models.JsonModels;
using RateMe.View.UserControls;

namespace RateMe.View.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private string _email = string.Empty;
        private string _password = string.Empty;
        
        
        public MainWindow()
        {
            // SetProjectDirectory();
            InitializeComponent();
            
            WindowBarDockPanel bar = new(this);
            WindowGrid.Children.Add(bar);
            
            Config config = JsonModelsHandler.GetConfig();
            OpenNextWin(config);
            
            Close();
        }
        

        private static void OpenNextWin(Config? config)
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
                return;
            }

            SyllabusModel syllabus = JsonModelsHandler.GetSyllabus();
            GradesWin gradesWin = new GradesWin(syllabus);
            gradesWin.Show();
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


        private void EmailEntered(object sender, TextChangedEventArgs e)
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

        private void PasswordEntered(object sender, TextChangedEventArgs e)
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