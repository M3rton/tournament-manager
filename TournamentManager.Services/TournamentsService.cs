using SixLabors.ImageSharp;
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
        return (await _unitOfWork.TournamentsRepository.GetAsync(t => t.Name == tournamentName)).FirstOrDefault() == null;
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

    public async Task<IEnumerable<Match>> GenerateBracketAsync(Tournament tournament)
    {
        switch (tournament.Strategy)
        {
            case StrategyType.Spider:
                return await GenerateSpiderBracketAsync(tournament);
            case StrategyType.Groups:
                return await GenerateGroupsBracketAsync(tournament);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<IEnumerable<Match>> GenerateSpiderBracketAsync(Tournament tournament)
    {
        IEnumerable<Match> CreateMatches(int roundSize, IEnumerable<Team?> teams, int currentRound)
        {
            var newMatches = new List<Match>();

            int matchNumber = 1;
            var teamsEnumerator = teams.GetEnumerator();
            while (teamsEnumerator.MoveNext())
            {
                Team? firstTeam = teamsEnumerator.Current;
                if (firstTeam == null)
                {
                    return Enumerable.Empty<Match>();
                }

                Team? secondTeam = null;
                if (teamsEnumerator.MoveNext())
                {
                    secondTeam = teamsEnumerator.Current;
                }

                Match match = new Match
                {
                    Round = currentRound,
                    MatchNumber = matchNumber,
                    FirstTeam = firstTeam,
                    SecondTeam = secondTeam,
                    WinnerTeam = secondTeam == null ? firstTeam : null,
                    IsFinished = secondTeam == null ? true : false
                };

                newMatches.Add(match);
                matchNumber++;
            }

            return newMatches;
        }

        async Task SaveMatches(IEnumerable<Match> matches)
        {
            foreach (var match in matches)
            {
                await _unitOfWork.MatchesRepository.AddAsync(match);

                match.FirstTeam.Matches.Add(match);
                _unitOfWork.TeamsRepository.Update(match.FirstTeam);

                if (match.SecondTeam != null)
                {
                    match.SecondTeam.Matches.Add(match);
                    _unitOfWork.TeamsRepository.Update(match.SecondTeam);
                }

                tournament.Matches.Add(match);
                _unitOfWork.TournamentsRepository.Update(tournament);

                await _unitOfWork.SaveAsync();
            }
        }

        IEnumerable<Match> newMatches;
        if (tournament.Matches.Count == 0)
        {
            int teamsCount = tournament.Teams.Count;

            if (teamsCount < 2)
            {
                return Enumerable.Empty<Match>();
            }

            int bracketSize = 2;
            while (bracketSize < teamsCount)
            {
                bracketSize *= 2;
            }

            IList<Team?> teams = new List<Team?>();

            for (int i = 0; i < bracketSize / 2; i++)
            {
                teams.Add(tournament.Teams[i]);
                teams.Add((bracketSize - 1 - i) >= tournament.Teams.Count ? null : tournament.Teams[bracketSize - 1 - i]);
            }

            newMatches = CreateMatches(teams.Count(), teams, 1);
            await SaveMatches(newMatches);
        }
        else
        {
            int currentRound = tournament.Matches.Last().Round + 1;

            IEnumerable<Match> lastRoundMatches = tournament.Matches.Where(m => m.Round == currentRound - 1);

            if (lastRoundMatches.Any(m => !m.IsFinished) || lastRoundMatches.Count() == 1)
            {
                return Enumerable.Empty<Match>();
            }

            var lastRoundWinners = lastRoundMatches
                .OrderBy(m => m.MatchNumber)
                .Select(m => m.WinnerTeam);

            var lastRoundWinnerEnumerator = lastRoundWinners.GetEnumerator();

            newMatches = CreateMatches(lastRoundWinners.Count() / 2, lastRoundWinners, currentRound);
            await SaveMatches(newMatches);
        }

        return newMatches;
    }

    private async Task<IEnumerable<Match>> GenerateGroupsBracketAsync(Tournament tournament)
    {
        throw new NotImplementedException();
    }

    public async Task ExportTournament(Tournament tournament)
    {

    }
}
