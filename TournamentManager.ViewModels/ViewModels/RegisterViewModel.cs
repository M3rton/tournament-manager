using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Navigation;
using TournamentManager.Core.Interfaces.Services;
using TournamentManager.ViewModels.Interfaces;

namespace TournamentManager.ViewModels.ViewModels
{
    public partial class RegisterViewModel : ObservableObject, ILoginViewModel
    {
        private readonly IViewModelFactory<LoginViewModel> _loginFactory;
        private readonly IUsersService _usersService;
        private readonly IWindowManager _windowManager;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        private string? _userName;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        private string? _password;

        public event EventHandler<User>? SignInSuccesful;

        public RegisterViewModel(IViewModelFactory<LoginViewModel> loginFactory,
            IUsersService usersService, IWindowManager windowManager)
        {
            _loginFactory = loginFactory;
            _usersService = usersService;
            _windowManager = windowManager;
        }

        [RelayCommand(CanExecute = nameof(CanRegister))]
        private async Task Register()
        {
            string message;

            if (await _usersService.CanRegisterAsync(UserName!, Password!))
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

            _windowManager.ShowWindow<PopUpWindowViewModel>(x => x.Message = message);
        }

        private bool CanRegister()
        {
            return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
        }

        public event EventHandler<ILoginViewModel>? ChangeViewModel;

        [RelayCommand]
        private void ChangeView()
        {
            ChangeViewModel?.Invoke(this, _loginFactory.Create());
        }
    }
}
