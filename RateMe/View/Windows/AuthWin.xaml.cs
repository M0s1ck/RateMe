using RateMe.Models.ClientModels;
using System.Windows;
using RateMe.Services;

namespace RateMe.View.Windows;

/// <summary>
/// Логика взаимодействия для AuthenticationWin.xaml
/// </summary>
public partial class AuthWin : BaseFullWin
{
    public DataHintTextModel LogInEmailModel { get; } = new("Email");
    public DataHintTextModel LogInPassModel { get; } = new("Password");

    public DataHintTextModel SignUpEmailModel { get; } = new("Email");
    public DataHintTextModel SignUpPassModel  { get; } = new("Password");
    public DataHintTextModel NameModel  { get; } = new("Имя");
    public DataHintTextModel SurnameModel { get; } = new("Фамилия");
        
    private bool _isLogIn;
    private readonly UserService _userService;

    private static readonly string[] AuTasks = ["Войти", "Sign up"];
    private static readonly string[] Questions = ["Уже есть аккаунт?", "Нет аккаунта?"];

    public AuthWin(UserService userService)
    {
        InitializeComponent();
        DataContext = this;
        _userService = userService;
            
        _isLogIn = false;
        FlipTaskButton.TheContent = _isLogIn ? AuTasks[0] : AuTasks[1];
        QuestionText.Text = _isLogIn ? Questions[0] : Questions[1];
            
        Loaded += (_, _) => AddHeaderBar(windowGrid);
    }

        
    /// <summary>
    /// User is added to remote bd.
    /// If success, subjects are added too.
    /// </summary>
    private async void OnSignUpClick(object sender, RoutedEventArgs e)
    {
        SignUpButton.IsEnabled = false;
        
        if (_userService.IsRemoteAlive)
        {
            await _userService.SignUp(SignUpEmailModel.Data, SignUpPassModel.Data, NameModel.Data, SurnameModel.Data);
        }
        else
        {
            MessageBox.Show("К сожалению сервер сейчас не доступен(");
        }
        
        SignUpButton.IsEnabled = true;
    }
        

    private async void OnSignInClick(object sender, RoutedEventArgs e)
    {
        SignInButton.IsEnabled = false;
        
        if (_userService.IsRemoteAlive)
        {
            await _userService.SignIn(LogInEmailModel.Data, LogInPassModel.Data);
        }
        else
        {
            MessageBox.Show("К сожалению сервер сейчас не доступен(");
        }
        
        SignInButton.IsEnabled = true;
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