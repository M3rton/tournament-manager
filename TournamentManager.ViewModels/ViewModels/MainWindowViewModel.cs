using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Services;
using TournamentManager.ViewModels.Interfaces;

namespace TournamentManager.ViewModels.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IDisposable
{
    private readonly IViewModelFactory<UpcomingTournamentsViewModel> _upcommingTournaments;
    private readonly IViewModelFactory<MyTournamentsViewModel> _myTournaments;
    private readonly IViewModelFactory<MyTeamViewModel> _myTeam;
    private readonly IViewModelFactory<CreateTeamViewModel> _createTeam;
    private readonly IViewModelFactory<MyAccountViewModel> _myAccount;
    private readonly IUsersService _usersService;

    public User? User { get; set; }

    [ObservableProperty]
    private Type? _currentViewModelType;

    private UserViewModel? _currentViewModel;
    public UserViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (_currentViewModel != null)
            {
                _currentViewModel.ChangeViewModel -= OnChangeViewModel;
            }
            SetProperty(ref _currentViewModel, value);
            if (_currentViewModel != null)
            {
                _currentViewModel.ChangeViewModel += OnChangeViewModel;
                _currentViewModel.User = User;
                CurrentViewModelType = _currentViewModel.GetType();
            }
        }
    }
    public MainWindowViewModel(
        IViewModelFactory<UpcomingTournamentsViewModel> upcommingTournaments,
        IViewModelFactory<MyTournamentsViewModel> myTournaments,
        IViewModelFactory<MyTeamViewModel> myTeam,
        IViewModelFactory<CreateTeamViewModel> createTeam,
        IViewModelFactory<MyAccountViewModel> myAccount,
        IUsersService userService
        )
    {
        _upcommingTournaments = upcommingTournaments;
        _myTournaments = myTournaments;
        _myTeam = myTeam;
        _createTeam = createTeam;
        _myAccount = myAccount;
        _usersService = userService;
    }

    private void OnChangeViewModel(object? sender, UserViewModel newViewModel)
    {
        CurrentViewModel = newViewModel;
    }

    [RelayCommand]
    private void OpenUpcommingTournaments()
    {
        CurrentViewModel = _upcommingTournaments.Create();
    }

    [RelayCommand]
    private void OpenMyTournaments()
    {
        CurrentViewModel = _myTournaments.Create();
    }

    [RelayCommand]
    private async Task OpenMyTeam()
    {
        await _usersService.LoadUserTeam(User!);

        if (User!.Account.Team == null)
        {
            CurrentViewModel = _createTeam.Create();
        }
        else
        {
            var myTeamView = _myTeam.Create();
            myTeamView.Team = User.Account.Team;
            CurrentViewModel = myTeamView;
        }
    }

    [RelayCommand]
    private async Task OpenMyAccount()
    {
        await _usersService.LoadUserAccount(User!);
        CurrentViewModel = _myAccount.Create();
    }

    public void Dispose()
    {
        if (_currentViewModel != null)
        {
            _currentViewModel.ChangeViewModel -= OnChangeViewModel;
        }
    }
}
