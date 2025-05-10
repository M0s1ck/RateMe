using RateMe.Api;
using RateMe.Models.ClientModels;
using RateMe.Models.DtoModels;
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
        public DataHintTextModel LogInEmailModel { get; }
        public DataHintTextModel LogInPassModel { get; }

        public DataHintTextModel SignUpEmailModel { get; }
        public DataHintTextModel SignUpPassModel { get; }
        public DataHintTextModel NameModel { get; }
        public DataHintTextModel SurnameModel { get; }


        private bool _isLogIn;
        private UserApi _userApi;

        private static readonly string[] AuTasks = ["Войти", "Sign up"];
        private static readonly string[] Questions = ["Уже есть акаунт?", "Нет аккаунта?"];

        public AuthWin()
        {
            InitializeComponent();
            WindowBarDockPanel bar = new(this);
            windowGrid.Children.Add(bar);

            DataContext = this;
            LogInEmailModel = new DataHintTextModel(string.Empty, "Email", Visibility.Visible);
            LogInPassModel = new DataHintTextModel(string.Empty, "Password", Visibility.Visible);

            SignUpEmailModel = new DataHintTextModel(string.Empty, "Email", Visibility.Visible);
            SignUpPassModel = new DataHintTextModel(string.Empty, "Password", Visibility.Visible);
            NameModel = new DataHintTextModel(string.Empty, "Имя", Visibility.Visible);
            SurnameModel = new DataHintTextModel(string.Empty, "Фамилия", Visibility.Visible);

            _isLogIn = false;
            FlipTaskButton.TheContent = _isLogIn ? AuTasks[0] : AuTasks[1];
            QuestionText.Text = _isLogIn ? Questions[0] : Questions[1];

            _userApi = new UserApi();
        }


        private async void OnSignUpClick(object sender, RoutedEventArgs e)
        {
            User user = new() { Email = SignUpEmailModel.Data, Password = SignUpPassModel.Data, Name = NameModel.Data, Surname = SurnameModel.Data};
            int? id = await _userApi.SignUpUserAsync(user);
            
            if (id != null)
            {
                MessageBox.Show($"You've been signed up! Your id: {id}");
            }
        }


        private async void OnLogInClick(object sender, RoutedEventArgs e)
        {
            AuthRequest request = new() { Email = LogInEmailModel.Data, Password = LogInPassModel.Data };
            User? user = await _userApi.AuthUserAsync(request);

            if (user != null)
            {
                MessageBox.Show($"Hello, {user.Name} {user.Surname}");
            }
        }


        private void OnFlipTaskClick(object sender, RoutedEventArgs e)
        {
            _isLogIn = !_isLogIn;
            FlipTaskButton.TheContent = _isLogIn ? AuTasks[0] : AuTasks[1];
            QuestionText.Text = _isLogIn ? Questions[0] : Questions[1];

            LogInPanel.Visibility = LogInPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            SignUpPanel.Visibility = LogInPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }


        private void OnWindowClick(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }
    }
}
