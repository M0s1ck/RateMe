using RateMe.Models.ClientModels;
using System.Windows;
using RateMe.Api.Clients;
using RateMe.Models.JsonModels;
using RateMe.Services;
using RateMeShared.Dto;

namespace RateMe.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthenticationWin.xaml
    /// </summary>
    public partial class AuthWin : BaseFullWin
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

        private static readonly string[] AuTasks = ["Войти", "Sign up"];
        private static readonly string[] Questions = ["Уже есть акаунт?", "Нет аккаунта?"];

        public AuthWin(SubjectsService subjService)
        {
            InitializeComponent();

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
            _subjectsService = subjService;
            
            Loaded += (_, _) => AddHeaderBar(windowGrid);
        }

        /// <summary>
        /// User with all the local subjects is added to remote bd
        /// </summary>
        private async void OnSignUpClick(object sender, RoutedEventArgs e)
        {
            SignUpButton.IsEnabled = false;
            
            UserDto userDto = new() { Email = SignUpEmailModel.Data, Password = SignUpPassModel.Data, Name = NameModel.Data, Surname = SurnameModel.Data };
            int? id = await _userClient.SignUpUserAsync(userDto);
            
            if (id != null)
            {
                MessageBox.Show($"You've been signed up! Your id: {id}");
                await _subjectsService.PushAllSubjectsByUserId((int)id);
                
                User user = new User(userDto);
                user.Id = (int)id;
                JsonModelsHandler.SaveUser(user);
            }
            
            SignUpButton.IsEnabled = true;
        }

        private async void OnLogInClick(object sender, RoutedEventArgs e)
        {
            AuthRequest request = new() { Email = LogInEmailModel.Data, Password = LogInPassModel.Data };
            UserDto? userDto = await _userClient.AuthUserAsync(request);

            if (userDto != null)
            {
                User user = new User(userDto);
                MessageBox.Show($"Hello, {user.Name} {user.Surname}");
                JsonModelsHandler.SaveUser(user);
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
    }
}
