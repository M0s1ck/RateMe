using RateMe.DataUtils.Models;
using RateMe.View.UserControls;
using System.Windows;
using System.Windows.Input;

namespace RateMe.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthenticationWin.xaml
    /// </summary>
    public partial class AuthWin : Window
    {
        private static readonly string[] AuTasks = ["Войти", "Sign up"]; 

        public DataHintTextModel EmailModel { get; }
        public DataHintTextModel PassModel { get; }
        public DataHintTextModel SecondPassModel { get; }

        private string _auTask;
        private bool _isLogIn;

        public AuthWin()
        {
            InitializeComponent();
            WindowBarDockPanel bar = new(this);
            windowGrid.Children.Add(bar);

            DataContext = this;
            EmailModel = new DataHintTextModel(string.Empty, "Email", Visibility.Visible);
            PassModel = new DataHintTextModel(string.Empty, "Password", Visibility.Visible);
            SecondPassModel = new DataHintTextModel(string.Empty, "Password", Visibility.Visible);

            _isLogIn = true;
            _auTask = _isLogIn ? AuTasks[0] : AuTasks[1];
            FlipTaskButton.Content = _auTask;
        }


        private void OnFlipTaskClick(object sender, RoutedEventArgs e)
        {
            _isLogIn = !_isLogIn;
            _auTask = _isLogIn ? AuTasks[0] : AuTasks[1];
            FlipTaskButton.Content = _auTask;

            LogInPanel.Visibility = LogInPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }


        private void OnWindowClick(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }
    }
}
