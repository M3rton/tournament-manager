using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Enums;
using TournamentManager.Core.Events;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.ViewModels.ViewModels;

public partial class CreateTournamentViewModel : ObservableObject
{
    public Player? Player { get; set; }

    private readonly ITournamentsService _tournamentsService;

    public ObservableCollection<StrategyType> Strategies { get; set; } =
        [.. Enum.GetValues(typeof(StrategyType)).Cast<StrategyType>()];

    public ObservableCollection<int> MaxTeams { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateTournamentCommand))]
    private string? _tournamentName;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateTournamentCommand))]
    private StrategyType? _selectedStrategy;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateTournamentCommand))]
    private int? _selectedMaxTeams;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateTournamentCommand))]
    private string? _tournamentDescription;

    private readonly PopUpMessageEvent _popUpMessageEvent;
    private readonly ChangeViewModelEvent _changeViewModelEvent;

    public CreateTournamentViewModel(ITournamentsService tournamentsService, IEventAggregator eventAggregator)
    {
        _tournamentsService = tournamentsService;

        MaxTeams = new ObservableCollection<int>();
        for (int i = 4; i <= 64; i*=2)
        {
            MaxTeams.Add(i);
        }

        _popUpMessageEvent = eventAggregator.GetEvent<PopUpMessageEvent>();
        _changeViewModelEvent = eventAggregator.GetEvent<ChangeViewModelEvent>();
    }

    [RelayCommand (CanExecute = nameof(CanCreateTournament))]
    private async Task CreateTournament()
    {
        if (Player == null || Player.Tournament != null)
        {
            return;
        }

        if (TournamentName == null || SelectedStrategy == null || SelectedMaxTeams == null)
        {
            return;
        }

        string message;
        bool teamCreated = false;

        if (await _tournamentsService.CanCreateTournamentAsync(TournamentName))
        {
            await _tournamentsService.CreateTournamentAsync(
                TournamentName,
                (StrategyType) SelectedStrategy,
                (int) SelectedMaxTeams,
                TournamentDescription,
                Player);

            message = "Successfully created new tournament.";

            teamCreated = true;
        }
        else
        {
            message = "Failed to create a tournament. This tournament name is already used.";
        }

        _popUpMessageEvent.Publish(new PopUpMessagePayload
        {
            Sender = this,
            Message = message
        });

        if (teamCreated)
        {
            _changeViewModelEvent.Publish(new ChangeViewModelPayload
            {
                Sender = this,
                ViewModelName = nameof(MyTournamentViewModel)
            });
        }
    }

    private bool CanCreateTournament()
    {
        if (string.IsNullOrEmpty(TournamentName) || SelectedMaxTeams == null || SelectedStrategy == null)
        {
            return false;
        }

        return true;
    }
}
