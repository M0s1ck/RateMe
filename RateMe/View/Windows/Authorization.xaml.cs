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
using RateMe.DataUtils.Models;
using RateMe.View.Windows;
using Path = System.IO.Path;

namespace RateMe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _email = string.Empty;
        private string _password = string.Empty;

        #region StaticConsts
        private static readonly string DataDir = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        private static readonly string SyllabusJsonPath = Path.Combine(DataDir, "syllabus.json");
        private static readonly string ConfigJsonPath = Path.Combine(DataDir, "config.json");
        #endregion
        

        public MainWindow()
        {
            SetProjectDirectory();
            InitializeComponent();
            
            WindowBarDockPanel bar = new(this);
            WindowGrid.Children.Add(bar);
            
            Config? config = JsonModelsHandler.GetConfig();
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

        private static void SetProjectDirectory()
        {
            string defaultPath = Directory.GetCurrentDirectory();
            string[] defaultPathArr = defaultPath.Split(System.IO.Path.DirectorySeparatorChar);
            string projectPath = string.Join(System.IO.Path.DirectorySeparatorChar, defaultPathArr[..^3]);
            Directory.SetCurrentDirectory(projectPath);
        }

    }
}