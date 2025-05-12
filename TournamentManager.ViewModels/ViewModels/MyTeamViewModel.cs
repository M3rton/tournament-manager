using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Events;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.ViewModels.ViewModels;

public partial class MyTeamViewModel : ObservableObject
{
    public Player? Player { get; set; }

    private readonly ITeamsService _teams;
    private readonly IPlayersService _players;

    private readonly ChangeViewModelEvent _changeViewModelEvent;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddPlayerCommand))]
    private string? _playerName;

    public MyTeamViewModel(ITeamsService teams, IPlayersService players, IEventAggregator eventAggregator)
    {
        _teams = teams;
        _players = players;

        _changeViewModelEvent = eventAggregator.GetEvent<ChangeViewModelEvent>();
    }

    [RelayCommand(CanExecute = nameof(CanAddPlayer))]
    private async Task AddPlayer()
    {
        if (Player == null || Player.Team == null || Player != Player.Team.TeamCaptain)
        {
            return;
        }

        await _teams.JoinTeamAsync(Player.Team!, PlayerName!);
    }

    private bool CanAddPlayer()
    {
        return !string.IsNullOrEmpty(PlayerName);
    }

    [RelayCommand]
    private async Task LeaveTeam()
    {
        await _players.LeaveTeamAsync(Player!);

        _changeViewModelEvent.Publish(new ChangeViewModelPayload
        {
            Sender = this,
            ViewModelName = nameof(CreateTeamViewModel),
        });
    }
}
