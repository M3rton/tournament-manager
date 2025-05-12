using CommunityToolkit.Mvvm.ComponentModel;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Events;
using TournamentManager.Core.Interfaces.Navigation;
using TournamentManager.ViewModels.Interfaces;

namespace TournamentManager.ViewModels.ViewModels;

public partial class LoginWindowViewModel : ObservableObject, IDisposable
{
    private readonly IViewModelFactory<LoginViewModel> _loginFactory;
    private readonly IViewModelFactory<RegisterViewModel> _registerFactory;
    private readonly IWindowManager _windowManager;

    private readonly SignedInEvent _signedEvent;
    private readonly ChangeViewModelEvent _changeViewModelEvent;
    private readonly PopUpMessageEvent _popUpMessageEvent;

    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    private readonly Dictionary<string, Action> _changeViewModelMap;

    public LoginWindowViewModel(
        IViewModelFactory<LoginViewModel> loginFactory,
        IViewModelFactory<RegisterViewModel> registerFactory,
        IEventAggregator eventAggregator,
        IWindowManager windowManager)
    {
        _windowManager = windowManager;
        _loginFactory = loginFactory;
        _registerFactory = registerFactory;

        _signedEvent = eventAggregator.GetEvent<SignedInEvent>();
        _changeViewModelEvent = eventAggregator.GetEvent<ChangeViewModelEvent>();
        _popUpMessageEvent = eventAggregator.GetEvent<PopUpMessageEvent>();

        _signedEvent.Subscribe(OnSignedIn);
        _changeViewModelEvent.Subscribe(OnChangeViewModel);
        _popUpMessageEvent.Subscribe(OnPopUpMessage);

        _changeViewModelMap = CreateMap();
        CurrentViewModel = loginFactory.Create();
    }

    private Dictionary<string, Action> CreateMap()
    {
        return new()
        {
            { "LoginViewModel", () => CurrentViewModel = _loginFactory.Create() },
            { "RegisterViewModel", () => CurrentViewModel = _registerFactory.Create() }
        };
    }

    public void OnPopUpMessage(PopUpMessagePayload payload)
    {
        if (payload.Sender == CurrentViewModel)
        {
            _windowManager.ShowWindow<PopUpWindowViewModel>(x => x.Message = payload.Message, this);
        }
    }

    public void OnSignedIn(User user)
    {
        _windowManager.ShowWindow<MainWindowViewModel>(x => x.User = user);
        _windowManager.CloseWindow(this);
    }

    public void OnChangeViewModel(ChangeViewModelPayload payload)
    {
        if (payload.Sender == CurrentViewModel && _changeViewModelMap.ContainsKey(payload.ViewModelName))
        {
            _changeViewModelMap[payload.ViewModelName]();
        }
    }

    public void Dispose()
    {
        _signedEvent.Unsubscribe(OnSignedIn);
        _changeViewModelEvent.Unsubscribe(OnChangeViewModel);
        _popUpMessageEvent.Unsubscribe(OnPopUpMessage);
    }
}
