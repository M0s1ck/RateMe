using RateMe.Services;

namespace RateMe.ViewModels;

public class AccountViewModel
{
    public UserService UserService { get; }

    public AccountViewModel(UserService userService)
    {
        UserService = userService;
    }
}