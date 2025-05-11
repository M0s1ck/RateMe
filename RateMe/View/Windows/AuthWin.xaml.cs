using RateMe.Api;
using RateMe.Models.ClientModels;
using RateMe.View.UserControls;
using System.Windows;
using System.Windows.Input;
using RateMe.Api.Clients;
using RateMe.Api.Services;
using RateMeShared.Dto;

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
        private readonly UserClient _userClient;

        private SubjectsService _subjectsService;
        // private readonly SubjectsApi _subjectsApi;

        private static readonly string[] AuTasks = ["Войти", "Sign up"];
        private static readonly string[] Questions = ["Уже есть акаунт?", "Нет аккаунта?"];

        public AuthWin()
        {
            InitializeComponent();
            WindowBarDockPanel bar = new(this);
            windowGrid.Children.Add(bar);

            DataContext = this;
            LogInEmailModel = new DataHintTextModel("Email");
            LogInPassModel = new DataHintTextModel("Password");

            SignUpEmailModel = new DataHintTextModel("Email");
            SignUpPassModel = new DataHintTextModel("Password");
            NameModel = new DataHintTextModel("Имя");
            SurnameModel = new DataHintTextModel("Фамилия");

            _isLogIn = false;
            FlipTaskButton.TheContent = _isLogIn ? AuTasks[0] : AuTasks[1];
            QuestionText.Text = _isLogIn ? Questions[0] : Questions[1];
            
            _userClient = new UserClient();
            
            SubjectsClient subjectsClient = new SubjectsClient();
            _subjectsService = new SubjectsService(subjectsClient);
        }


        private async void OnSignUpClick(object sender, RoutedEventArgs e)
        {
            SignUpButton.IsEnabled = false;
            
            UserDto userDto = new() { Email = SignUpEmailModel.Data, Password = SignUpPassModel.Data, Name = NameModel.Data, Surname = SurnameModel.Data};
            int? id = await _userClient.SignUpUserAsync(userDto);
            
            if (id != null)
            {
                MessageBox.Show($"You've been signed up! Your id: {id}");
                await _subjectsService.PushSubjectsByUserId((int)id);
            }
            
            SignUpButton.IsEnabled = true;
        }


        private async void OnLogInClick(object sender, RoutedEventArgs e)
        {
            AuthRequest request = new() { Email = LogInEmailModel.Data, Password = LogInPassModel.Data };
            UserDto? user = await _userClient.AuthUserAsync(request);

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
