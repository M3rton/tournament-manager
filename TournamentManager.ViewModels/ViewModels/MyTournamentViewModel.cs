using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.ObjectModel;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Navigation;
using TournamentManager.Core.Interfaces.Services;
using TournamentManager.ViewModels.Options;
using TournamentManager.ViewModels.Utilities;

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
    private readonly IMatchesService _matchesService;
    private readonly IWindowService _windowManager;
    private readonly IBracketGenerationService _bracketGenerationService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddTeamCommand))]
    private string? _teamName;

    public MyTournamentViewModel(ITournamentsService tournamentsService, IMatchesService matchesService, IWindowService windowManager, IBracketGenerationService bracketGenerationService)
    {
        _tournamentsService = tournamentsService;
        _matchesService = matchesService;
        _windowManager = windowManager;
        _bracketGenerationService = bracketGenerationService;
    }

    [RelayCommand]
    private async Task GenerateBracket()
    {
        if (Player == null || Player.Tournament == null || Player != Player.Tournament.Owner)
        {
            return;
        }
        var newMatches = await _bracketGenerationService.GenerateBracketAsync(Player.Tournament);

        CurrentMatches.AddRange(newMatches.Where(m => !m.IsFinished));
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
        else if (match.FirstTeamWins > match.SecondTeamWins || match.SecondTeam == null)
        {
            await _matchesService.SaveWinnerAsync(match, match.FirstTeam);
        }
        else
        {
            await _matchesService.SaveWinnerAsync(match, match.SecondTeam);
        }

        CurrentMatches.Remove(match);
        var newMatches = await _bracketGenerationService.GenerateBracketAsync(Player.Tournament);

        CurrentMatches.AddRange(newMatches.Where(m => !m.IsFinished));

        if (CurrentMatches.Count == 0)
        {
            await _tournamentsService.SaveWinnerAsync(
                Player.Tournament, Player.Tournament.Matches.Last().WinnerTeam!);

            Winner = Player.Tournament.Winner;
        }
    }

    [RelayCommand(CanExecute = nameof(CanAddTeam))]
    private async Task AddTeam()
    {
        if (Player == null || Player.Tournament == null)
        {
            return;
        }

        if (Player != Player.Tournament.Owner || Player.Tournament.Matches.Count >= Player.Tournament.MaxTeams)
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
    private async Task ExportTournament()
    {
        if (Player == null || Player.Tournament == null)
        {
            return;
        }

        var optionsBuilder = new ExportBracketAsImageOptionsBuilder();

        string? saveTo = await _windowManager.ShowSaveFileDialog("Save Tournament", "TXT|*.txt", $"{Player.Tournament.Name}_bracket_export");

        if (string.IsNullOrEmpty(saveTo))
        {
            return;
        }

        using var stream = new FileStream(saveTo, FileMode.Create);
        using var writer = new StreamWriter(stream);

        await _tournamentsService.ExportTournamentAsync(Player.Tournament, writer);
    }

    [RelayCommand]
    private async Task ExportBracket()
    {
        if (Player == null || Player.Tournament == null)
        {
            return;
        }

        var optionsBuilder = new ExportBracketAsImageOptionsBuilder();

        using Image<Rgba32> image = await ExportBracketUtils.ExportBracketAsImageAsync(Player.Tournament, optionsBuilder.Build());

        string? saveTo = await _windowManager.ShowSaveFileDialog("Save Bracket as Image", "BMP|*.bmp|PNG|*.png|JPEG|*.jpeg|GIF|*.gif", $"{Player.Tournament.Name}_bracket_export");

        if (string.IsNullOrEmpty(saveTo))
        {
            return;
        }

        await image.SaveAsync(saveTo);
    }
}
