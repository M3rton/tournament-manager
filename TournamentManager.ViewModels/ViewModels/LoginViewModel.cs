using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Navigation;
using TournamentManager.Core.Interfaces.Services;
using TournamentManager.ViewModels.Interfaces;

namespace TournamentManager.ViewModels.ViewModels;

public partial class LoginViewModel : ObservableObject, ILoginViewModel
{
    private readonly IViewModelFactory<RegisterViewModel> _registerFactory;
    private readonly IUsersService _usersService;
    private readonly IWindowManager _windowManager;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
    private string? _userName;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
    private string? _password;

    public LoginViewModel(IViewModelFactory<RegisterViewModel> registerFactory,
        IUsersService usersService, IWindowManager windowManager)
    {
        _registerFactory = registerFactory;
        _usersService = usersService;
        _windowManager = windowManager;
    }

    public event EventHandler<User>? SignInSuccesful;

    [RelayCommand(CanExecute = nameof(CanSingIn))]
    private async Task SignIn()
    {
        User? user = await _usersService.LoginAsync(UserName!, Password!);

        if (user != null)
        {
            SignInSuccesful?.Invoke(this, user);
        }
        else
        {
            _windowManager.ShowWindow<PopUpWindowViewModel>
                (
                    x => x.Message = "Incorrect user name or password."
                );
        }

        UserName = "";
        Password = "";
    }

    private bool CanSingIn()
    {
        return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
    }

    public event EventHandler<ILoginViewModel>? ChangeViewModel;

    [RelayCommand]
    private void ChangeView()
    {
        ChangeViewModel?.Invoke(this, _registerFactory.Create());
    }
}
