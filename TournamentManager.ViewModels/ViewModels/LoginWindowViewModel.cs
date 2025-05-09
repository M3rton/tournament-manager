using CommunityToolkit.Mvvm.ComponentModel;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Navigation;
using TournamentManager.ViewModels.Interfaces;

namespace TournamentManager.ViewModels.ViewModels;

public class LoginWindowViewModel : ObservableObject, IDisposable
{
    private readonly IWindowManager _windowManager;

    private ILoginViewModel? _currentViewModel;
    public ILoginViewModel? CurrentViewModel
    {
        get { return _currentViewModel; } 
        private set
        {
            if (_currentViewModel != null)
            {
                _currentViewModel.ChangeViewModel -= OnChangeViewModel;
                _currentViewModel.SignInSuccesful -= SignedIn;
            }
            SetProperty(ref _currentViewModel, value);
            if (_currentViewModel != null)
            {
                _currentViewModel.ChangeViewModel += OnChangeViewModel;
                _currentViewModel.SignInSuccesful += SignedIn;
            }
        }
    }

    public LoginWindowViewModel(LoginViewModel loginViewModel, IWindowManager windowManager)
    {
        CurrentViewModel = loginViewModel;
        _windowManager = windowManager;
    }

    public void SignedIn(object? sender, User user)
    {
        _windowManager.ShowWindow<MainWindowViewModel>(x => x.User = user);
        _windowManager.CloseWindow(this);
    }

    public void OnChangeViewModel(object? sender, ILoginViewModel newViewModel)
    {
        CurrentViewModel = newViewModel;
    }

    public void Dispose()
    {
        if (_currentViewModel != null)
        {
            _currentViewModel.ChangeViewModel -= OnChangeViewModel;
        }
    }
}
