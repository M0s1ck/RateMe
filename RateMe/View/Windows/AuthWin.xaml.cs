using RateMe.Models.ClientModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RateMe.Services;
using RateMe.View.UserControls;

namespace RateMe.View.Windows;

/// <summary>
/// Логика взаимодействия для AuthWin.xaml
/// </summary>
public partial class AuthWin : BaseFullWin
{
    public event Action? UserSignedUp;
    
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
        Close();
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
        UserSignedUp?.Invoke();
        Close();
    }
        

    private void OnFlipTaskClick(object sender, RoutedEventArgs e)
    {
        _isLogIn = !_isLogIn;
        FlipTaskButton.TheContent = _isLogIn ? AuTasks[0] : AuTasks[1];
        QuestionText.Text = _isLogIn ? Questions[0] : Questions[1];

        LogInPanel.Visibility = LogInPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        SignUpPanel.Visibility = LogInPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
    }
    
    // Arrow management

    private void OnSignInKeyPressed(object sender, KeyEventArgs e)
    {
        DataHintTextBox box = (DataHintTextBox)sender;
        DependencyObject panel = VisualTreeHelper.GetParent(box)!;
        
        if (box.Name == "LogInEmailForm" && e.Key == Key.Down)
        {
            DataHintTextBox next = (DataHintTextBox)VisualTreeHelper.GetChild(panel, 1);
            next.DataTextBox.Focus();
        } 
        else if (box.Name == "LogInPassForm" && e.Key == Key.Up)
        {
            DataHintTextBox next = (DataHintTextBox)VisualTreeHelper.GetChild(panel, 0);
            next.DataTextBox.Focus();
        }
    }
    
    private void OnSignUpKeyPressed(object sender, KeyEventArgs e)
    {
        DataHintTextBox box = (DataHintTextBox)sender;
        DependencyObject panel = VisualTreeHelper.GetParent(box)!;
        
        if (box.Name == "SignUpNameForm" && e.Key == Key.Right && box.DataTextBox.CaretIndex == box.DataTextBox.Text.Length)
        {
            DataHintTextBox next = (DataHintTextBox)VisualTreeHelper.GetChild(panel, 1);
            next.DataTextBox.Focus();
            return;
        }
        
        if (box.Name == "SignUpSurnameForm" && e.Key == Key.Left && box.DataTextBox.CaretIndex == 0)
        {
            DataHintTextBox next = (DataHintTextBox)VisualTreeHelper.GetChild(panel, 0);
            next.DataTextBox.Focus();
            return;
        }

        if (box.Name is "SignUpSurnameForm" or "SignUpNameForm" && e.Key == Key.Up)
        {
            DependencyObject prePanel = VisualTreeHelper.GetParent(panel)!;
            DataHintTextBox next = (DataHintTextBox)VisualTreeHelper.GetChild(prePanel, 0);
            next.DataTextBox.Focus();
            return;
        }
        
        if (box.Name is "SignUpSurnameForm" or "SignUpNameForm" && e.Key == Key.Down)
        {
            DependencyObject prePanel = VisualTreeHelper.GetParent(panel)!;
            DataHintTextBox next = (DataHintTextBox)VisualTreeHelper.GetChild(prePanel, 2);
            next.DataTextBox.Focus();
            return;
        }
        
        if (box.Name == "SignUpEmailForm" && e.Key == Key.Down || box.Name == "SignUpPassForm" && e.Key == Key.Up)
        {
            DependencyObject childPanel = VisualTreeHelper.GetChild(panel, 1);
            DataHintTextBox next = (DataHintTextBox)VisualTreeHelper.GetChild(childPanel, 0);
            next.DataTextBox.Focus();
        }
    }
}