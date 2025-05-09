using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Services;
using TournamentManager.ViewModels.Interfaces;

namespace TournamentManager.ViewModels.ViewModels;

public partial class MyTeamViewModel : UserViewModel
{
    private readonly IViewModelFactory<CreateTeamViewModel> _createTeam;
    private readonly ITeamsService _teams;
    private readonly IPlayersService _players;

    [ObservableProperty]
    private Team? _team;

    [ObservableProperty]
    private string? _playerName;

    public MyTeamViewModel(IViewModelFactory<CreateTeamViewModel> createTeam, ITeamsService teams, IPlayersService players)
    {
        _createTeam = createTeam;
        _teams = teams;
        _players = players;
    }

    [RelayCommand(CanExecute = nameof(CanInvitePlayer))]
    private void AddPlayer()
    {

    }

    private bool CanInvitePlayer()
    {
        return !string.IsNullOrEmpty(PlayerName);
    }
}
