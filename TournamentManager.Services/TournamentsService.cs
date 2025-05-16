using System.Collections.ObjectModel;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Enums;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.Services;

internal class TournamentsService : ITournamentsService
{
    private readonly IUnitOfWork _unitOfWork;

    public TournamentsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateTournamentAsync(string tournamentName, StrategyType strategyType, int maxTeams, string? description, Player player)
    {
        if (!await CanCreateTournamentAsync(tournamentName) || player.Tournament != null)
        {
            return;
        }

        var tournament = new Tournament
        {
            Name = tournamentName,
            Strategy = strategyType,
            MaxTeams = maxTeams,
            Description = description,
            Owner = player,
            Teams = new ObservableCollection<Team>(),
            Matches = new ObservableCollection<Match>()

        };
        player.Tournament = tournament;

        _unitOfWork.PlayersRepository.Update(player);
        await _unitOfWork.TournamentsRepository.AddAsync(tournament);

        await _unitOfWork.SaveAsync();
    }

    public async Task<bool> CanCreateTournamentAsync(string tournamentName)
    {
        return (await _unitOfWork.TournamentsRepository.GetAsync(t => t.Name.ToLower() == tournamentName.ToLower())).FirstOrDefault() == null;
    }

    public async Task AddTeamAsync(Tournament tournament, string teamName)
    {
        Team? team = (await _unitOfWork.TeamsRepository.GetAsync(t => t.Name == teamName)).FirstOrDefault();

        if (team != null)
        {
            if (tournament.Teams.Contains(team) || tournament.Teams.Count >= tournament.MaxTeams)
            {
                return;
            }

            team.Tournaments.Add(tournament);
            tournament.Teams.Add(team);

            _unitOfWork.TeamsRepository.Update(team);
            _unitOfWork.TournamentsRepository.Update(tournament);

            await _unitOfWork.SaveAsync();
        }
    }

    public async Task SaveWinnerAsync(Tournament tournament, Team team)
    {
        tournament.Winner = team;

        _unitOfWork.TournamentsRepository.Update(tournament);
        await _unitOfWork.SaveAsync();
    }

    public Task ExportTournamentAsync(Tournament tournament, StreamWriter streamWriter) => Task.Run(() =>
    {
        switch (tournament.Strategy)
        {
            case StrategyType.Spider:
                ExportSpiderTournamentAsync(tournament, streamWriter);
                break;
            case StrategyType.DoubleElimination:
                ExportDoubleEliminationTournamentAsync(tournament, streamWriter);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    });

    private void ExportSpiderTournamentAsync(Tournament tournament, StreamWriter streamWriter)
    {
        foreach (var match in tournament.Matches)
        {
            if (match.FirstTeam == null || match.SecondTeam == null)
            {
                continue;
            }

            streamWriter.WriteLine($"Round {match.Round}: {match.FirstTeam?.Name} vs {match.SecondTeam?.Name}");
            if (match.WinnerTeam != null)
            {
                streamWriter.WriteLine($"Winner: {match.WinnerTeam.Name}");
            }
            else
            {
                streamWriter.WriteLine("Match not finished yet.");
            }
            streamWriter.WriteLine();
        }

        if (tournament.Winner != null)
        {
            streamWriter.WriteLine($"Tournament Winner: {tournament.Winner.Name}");
        }
    }

    private void ExportDoubleEliminationTournamentAsync(Tournament tournament, StreamWriter streamWriter)
    {
        var winnersBracket = tournament.Matches
            .Where(m => m.IsWinnersBracket)
            .ToList();
        var losersBracket = tournament.Matches
            .Where(m => !m.IsWinnersBracket)
            .ToList();

        streamWriter.WriteLine("Winners Bracket:");
        foreach (var match in winnersBracket)
        {
            if (match.FirstTeam == null || match.SecondTeam == null)
            {
                continue;
            }

            streamWriter.WriteLine($"Round {match.Round}: {match.FirstTeam?.Name} vs {match.SecondTeam?.Name}");
            if (match.WinnerTeam != null)
            {
                streamWriter.WriteLine($"Winner: {match.WinnerTeam.Name}");
            }
            else
            {
                streamWriter.WriteLine("Match not finished yet.");
            }
            streamWriter.WriteLine();
        }

        streamWriter.WriteLine("Losers Bracket:");
        foreach (var match in losersBracket)
        {
            if (match.FirstTeam == null || match.SecondTeam == null)
            {
                continue;
            }

            streamWriter.WriteLine($"Round {match.Round}: {match.FirstTeam?.Name} vs {match.SecondTeam?.Name}");
            if (match.WinnerTeam != null)
            {
                streamWriter.WriteLine($"Winner: {match.WinnerTeam.Name}");
            }
            else
            {
                streamWriter.WriteLine("Match not finished yet.");
            }
            streamWriter.WriteLine();
        }

        if (tournament.Winner != null)
        {
            streamWriter.WriteLine($"Tournament Winner: {tournament.Winner.Name}");
        }
    }
}
