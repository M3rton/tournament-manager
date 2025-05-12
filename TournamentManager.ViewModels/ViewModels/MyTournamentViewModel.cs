using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.ViewModels.ViewModels;

public partial class MyTournamentViewModel : ObservableObject
{
    public Player? Player { get; set; }

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
        await _tournamentsService.GenerateBracketAsync(Player.Tournament);
    }

    [RelayCommand]
    private async Task SaveMatch(Match match)
    {
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
        OnPropertyChanged();
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
