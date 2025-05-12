using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.ViewModels.ViewModels;

public partial class MyTournamentViewModel : ObservableObject
{
    private Player? _player;
    public Player? Player 
    { 
        get => _player;
        set
        {
            _player = value;
            if (_player != null && _player.Tournament != null)
            {
                Winner = _player.Tournament.Winner;
                CurrentMatches.AddRange(_player.Tournament.Matches.Where(m => !m.IsFinished));
            }
        }
    }

    [ObservableProperty]
    private Team? _winner;

    public ObservableCollection<Match> CurrentMatches { get; } = new ObservableCollection<Match>();

    private readonly ITournamentsService _tournamentsService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddTeamCommand))]
    private string? _teamName;

    public MyTournamentViewModel(ITournamentsService tournamentsService)
    {
        _tournamentsService = tournamentsService;
    }

    [RelayCommand]
    private async Task GenerateBracket()
    {
        if (Player!.Tournament == null || Player != Player.Tournament.Owner)
        {
            return;
        }
        CurrentMatches.AddRange(await _tournamentsService.GenerateBracketAsync(Player.Tournament));
    }

    [RelayCommand]
    private async Task SaveMatch(Match match)
    {
        if (Player == null || Player.Tournament == null)
        {
            return;
        }

        if (match.FirstTeamWins == match.SecondTeamWins)
        {
            return;
        }

        if (match.FirstTeamWins > match.SecondTeamWins)
        {
            match.WinnerTeam = match.FirstTeam;
        }
        else
        {
            match.WinnerTeam = match.SecondTeam;
        }

        match.IsFinished = true;
        CurrentMatches.Remove(match);
        CurrentMatches.AddRange(await _tournamentsService.GenerateBracketAsync(Player.Tournament));

        if (CurrentMatches.Count == 0)
        {
            await _tournamentsService.SaveWinnerAsync(
                Player.Tournament, Player.Tournament.Matches.Last().WinnerTeam!);

            Winner = Player.Tournament.Winner;
        }
    }

    [RelayCommand]
    private void ExportTournament()
    {

    }

    [RelayCommand(CanExecute = nameof(CanAddTeam))]
    private async Task AddTeam()
    {
        if (Player!.Tournament == null || Player != Player.Tournament.Owner || Player.Tournament.Matches.Count != 0)
        {
            return;
        }

        await _tournamentsService.AddTeamAsync(Player.Tournament, TeamName!);
    }

    private bool CanAddTeam()
    {
        return !string.IsNullOrEmpty(TeamName);
    }

    [RelayCommand]
    private void ExportBracket()
    {

    }
}
