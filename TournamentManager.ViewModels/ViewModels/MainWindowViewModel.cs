using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Events;
using TournamentManager.Core.Interfaces.Navigation;
using TournamentManager.ViewModels.Interfaces;

namespace TournamentManager.ViewModels.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IDisposable
{
    public User? User { get; set; }

    private readonly IViewModelFactory<MyTournamentViewModel> _myTournament;
    private readonly IViewModelFactory<CreateTournamentViewModel> _createTournament;
    private readonly IViewModelFactory<MyTeamViewModel> _myTeam;
    private readonly IViewModelFactory<CreateTeamViewModel> _createTeam;
    private readonly IViewModelFactory<MyAccountViewModel> _myAccount;
    private readonly IWindowService _windowManager;

    private readonly ChangeViewModelEvent _changeViewModelEvent;
    private readonly PopUpMessageEvent _popUpMessageEvent;

    [ObservableProperty]
    private Type? _currentViewModelType;

    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    private readonly Dictionary<string, Action> _changeViewModelMap;

    public MainWindowViewModel(
        IViewModelFactory<MyTournamentViewModel> myTournament,
        IViewModelFactory<CreateTournamentViewModel> createTournament,
        IViewModelFactory<MyTeamViewModel> myTeam,
        IViewModelFactory<CreateTeamViewModel> createTeam,
        IViewModelFactory<MyAccountViewModel> myAccount,
        IWindowService windowManager,
        IEventAggregator eventAggregator
        )
    {
        _myTournament = myTournament;
        _createTournament = createTournament;
        _myTeam = myTeam;
        _createTeam = createTeam;
        _myAccount = myAccount;
        _windowManager = windowManager;

        _changeViewModelEvent = eventAggregator.GetEvent<ChangeViewModelEvent>();
        _popUpMessageEvent = eventAggregator.GetEvent<PopUpMessageEvent>();

        _changeViewModelEvent.Subscribe(OnChangeViewModel);
        _popUpMessageEvent.Subscribe(OnPopUpMessage);

        _changeViewModelMap = CreateMap();
    }

    private Dictionary<string, Action> CreateMap()
    {
        return new()
        {
            { "MyTournamentViewModel", OpenMyTournament },
            { "CreateTournamentViewModel", OpenMyTournament },
            { "MyTeamViewModel", OpenMyTeam },
            { "CreateTeamViewModel", OpenMyTeam },
            { "MyAccountViewModel", OpenMyAccount },
        };
    }

    public void OnPopUpMessage(PopUpMessagePayload payload)
    {
        if (payload.Sender == CurrentViewModel)
        {
            _windowManager.ShowWindow<PopUpWindowViewModel>(x => x.Message = payload.Message, this);
        }
    }

    public void OnChangeViewModel(ChangeViewModelPayload payload)
    {
        if (payload.Sender == CurrentViewModel && _changeViewModelMap.TryGetValue(payload.ViewModelName, out var action))
        {
            action();
        }
    }

    [RelayCommand]
    private void OpenMyTournament()
    {
        if (User == null)
        {
            return;
        }

        ObservableObject newViewModel;

        if (User.Account.Tournament == null)
        {
            var createTournament= _createTournament.Create();
            createTournament.Player = User.Account;
            newViewModel = createTournament;
        }
        else
        {
            var myTournament = _myTournament.Create();
            myTournament.Player = User.Account;
            newViewModel = myTournament;
        }

        CurrentViewModelType = newViewModel.GetType();
        CurrentViewModel = newViewModel;
    }

    [RelayCommand]
    private void OpenMyTeam()
    {
        if (User == null)
        {
            return;
        }

        ObservableObject newViewModel;

        if (User.Account.Team == null)
        {
            var createTeam = _createTeam.Create();
            createTeam.Player = User.Account;
            newViewModel = createTeam;
        }
        else
        {
            var myTeam = _myTeam.Create();
            myTeam.Player = User.Account;
            newViewModel = myTeam;
        }

        CurrentViewModelType = newViewModel.GetType();
        CurrentViewModel = newViewModel;
    }

    [RelayCommand]
    private void OpenMyAccount()
    {
        if (User == null)
        {
            return;
        }

        var newViewModel = _myAccount.Create();

        newViewModel.Player = User.Account;
        CurrentViewModelType = newViewModel.GetType();
        CurrentViewModel = newViewModel;
    }

    public void Dispose()
    {
        _changeViewModelEvent.Unsubscribe(OnChangeViewModel);
        _popUpMessageEvent.Unsubscribe(OnPopUpMessage);
    }
}
