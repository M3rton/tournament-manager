using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Events;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.ViewModels.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUsersService _usersService;

    private readonly SignedInEvent _signedInEvent;
    private readonly ChangeViewModelEvent _changeViewModelEvent;
    private readonly PopUpMessageEvent _popUpMessageEvent;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
    private string? _userName;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
    private string? _password;

    public LoginViewModel(IUsersService usersService, IEventAggregator eventAggregator)
    {
        _usersService = usersService;

        _signedInEvent = eventAggregator.GetEvent<SignedInEvent>();
        _changeViewModelEvent = eventAggregator.GetEvent<ChangeViewModelEvent>();
        _popUpMessageEvent = eventAggregator.GetEvent<PopUpMessageEvent>();
    }

    [RelayCommand(CanExecute = nameof(CanSingIn))]
    private async Task SignIn()
    {
        User? user = await _usersService.LoginAsync(UserName!, Password!);

        if (user != null)
        {
            _signedInEvent.Publish(user);
        }
        else
        {
            _popUpMessageEvent.Publish(new PopUpMessagePayload
            {
                Sender = this,
                Message = "Invalid username or password."
            });
        }

        UserName = "";
        Password = "";
    }

    private bool CanSingIn()
    {
        return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
    }

    [RelayCommand]
    private void ChangeView()
    {
        _changeViewModelEvent.Publish(new ChangeViewModelPayload
        {
            Sender = this,
            ViewModelName = nameof(RegisterViewModel),
        });
    }
}
