using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TournamentManager.Core.Events;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.ViewModels.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IUsersService _usersService;

        private readonly ChangeViewModelEvent _changeViewModelEvent;
        private readonly PopUpMessageEvent _popUpMessageEvent;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        private string? _userName;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        private string? _password;

        public RegisterViewModel(IUsersService usersService, IEventAggregator eventAggregator)
        {
            _usersService = usersService;

            _changeViewModelEvent = eventAggregator.GetEvent<ChangeViewModelEvent>();
            _popUpMessageEvent = eventAggregator.GetEvent<PopUpMessageEvent>();
        }

        [RelayCommand(CanExecute = nameof(CanRegister))]
        private async Task Register()
        {
            string message;

            if (await _usersService.CanRegisterAsync(UserName!))
            {
                await _usersService.RegisterAsync(UserName!, Password!);
                message = "Successfully registered.";
            }
            else
            {
                message = "Registration failed. This user name is already used.";
            }

            UserName = "";
            Password = "";

            _popUpMessageEvent.Publish(new PopUpMessagePayload
            {
                Sender = this,
                Message = message
            });
        }

        private bool CanRegister()
        {
            return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
        }


        [RelayCommand]
        private void ChangeView()
        {
            _changeViewModelEvent.Publish(new ChangeViewModelPayload
            {
                Sender = this,
                ViewModelName = nameof(LoginViewModel)
            });
        }
    }
}
