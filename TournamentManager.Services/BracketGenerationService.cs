using TournamentManager.Core.Entities;
using TournamentManager.Core.Enums;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.Services;

internal class BracketGenerationService : IBracketGenerationService
{
    private readonly IUnitOfWork _unitOfWork;

    public BracketGenerationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Match>> GenerateBracketAsync(Tournament tournament)
    {
        switch (tournament.Strategy)
        {
            case StrategyType.Spider:
                return await GenerateSpiderBracketAsync(tournament);
            case StrategyType.DoubleElimination:
                return await GenerateDoubleEliminationBracketAsync(tournament);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<IEnumerable<Match>> GenerateSpiderBracketAsync(Tournament tournament)
    {
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

            newMatches = CreateMatches(teams, 1, true);
            await SaveMatches(tournament, newMatches);

            return newMatches;
        }

        int currentRound = tournament.Matches
            .Select(m => m.Round)
            .Max();

        if (tournament.Matches.Any(m => !m.IsFinished))
        {
            return Enumerable.Empty<Match>();
        }

        var lastRoundWinners = tournament.Matches
            .Where(m => m.Round == currentRound)
            .OrderBy(m => m.MatchNumber)
            .Select(m => m.WinnerTeam)
            .ToList();

        if (lastRoundWinners.Count == 1)
        {
            return Enumerable.Empty<Match>();
        }

        newMatches = CreateMatches(lastRoundWinners, currentRound + 1, true);
        await SaveMatches(tournament, newMatches);

        return newMatches;
    }

    private async Task<IEnumerable<Match>> GenerateDoubleEliminationBracketAsync(Tournament tournament)
    {
        var newMatches = new List<Match>();
        if (tournament.Matches.Count == 0)
        {
            int teamsCount = tournament.Teams.Count;

            if (teamsCount < 4)
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

            newMatches.AddRange(CreateMatches(teams, 1, true));
            await SaveMatches(tournament, newMatches);

            return newMatches;
        }

        int currentWinnersBracketRound = tournament.Matches
            .Where(m => m.IsWinnersBracket)
            .Select(m => m.Round)
            .Max();
        int currentLosersBracketRound = tournament.Matches
            .Where(m => !m.IsWinnersBracket)
            .Select(m => m.Round)
            .DefaultIfEmpty(0)
            .Max();

        if (tournament.Matches.Any(m => !m.IsFinished))
        {
            return Enumerable.Empty<Match>();
        }

        var lastWinnersBracketMatches = tournament.Matches
            .Where(m => m.Round == currentWinnersBracketRound && m.IsWinnersBracket)
            .ToList();
        var lastLosersBracketMatches = tournament.Matches
            .Where(m => m.Round == currentLosersBracketRound && !m.IsWinnersBracket)
            .ToList();

        if (currentWinnersBracketRound < currentLosersBracketRound)
        {
            var finalTeams = new List<Team?>
            {
                lastWinnersBracketMatches.First().WinnerTeam,
                lastLosersBracketMatches.First().WinnerTeam
            };
            newMatches.AddRange(CreateMatches(finalTeams, currentWinnersBracketRound + 1, true));
            await SaveMatches(tournament, newMatches);

            return newMatches;
        }

        var lastLosersMatch = lastLosersBracketMatches.FirstOrDefault();
        var lastWinnersMatch = lastWinnersBracketMatches.FirstOrDefault();

        if (lastLosersMatch != null && lastWinnersMatch != null &&
            (lastLosersMatch.WinnerTeam == lastWinnersMatch.FirstTeam ||
             lastLosersMatch.WinnerTeam == lastWinnersMatch.SecondTeam))
        {
            return Enumerable.Empty<Match>();
        }

        if (lastWinnersBracketMatches.Count() > 1)
        {
            var winnersBracketWinners = lastWinnersBracketMatches
                .OrderBy(m => m.MatchNumber)
                .Select(m => m.WinnerTeam)
                .ToList();

            newMatches.AddRange(CreateMatches(winnersBracketWinners, currentWinnersBracketRound + 1, true));
        }

        var winnersBracketLosers = lastWinnersBracketMatches
            .OrderBy(m => m.MatchNumber)
            .Select(m => m.WinnerTeam == m.FirstTeam ? m.SecondTeam : m.FirstTeam)
            .ToList();

        var losersBracketWinners = lastLosersBracketMatches
            .OrderBy(m => m.MatchNumber)
            .Select(m => m.WinnerTeam)
            .ToList();

        if (winnersBracketLosers.Count() < losersBracketWinners.Count())
        {
            newMatches.AddRange(CreateMatches(losersBracketWinners, currentLosersBracketRound + 1, false));
            await SaveMatches(tournament, newMatches);

            return newMatches;
        }

        var losersBracketTeams = new List<Team?>();
        for (int i = 0; i < winnersBracketLosers.Count(); i++)
        {
            losersBracketTeams.Add(winnersBracketLosers[i]);
            if (i < losersBracketWinners.Count())
            {
                losersBracketTeams.Add(losersBracketWinners[i]);
            }
        }

        newMatches.AddRange(CreateMatches(losersBracketTeams, currentLosersBracketRound + 1, false));
        await SaveMatches(tournament, newMatches);

        return newMatches;
    }

    private IEnumerable<Match> CreateMatches(IList<Team?> teams, int currentRound, bool isWinnersBracket)
    {
        var newMatches = new List<Match>();
        int matchNumber = 1;

        for (int i = 0; i < teams.Count; i += 2)
        {
            Team? firstTeam = teams[i];

            Team? secondTeam = null;
            if (i + 1 < teams.Count)
            {
                secondTeam = teams[i + 1];
            }

            Match match = new Match
            {
                Round = currentRound,
                MatchNumber = matchNumber,
                FirstTeam = firstTeam,
                SecondTeam = secondTeam,
                WinnerTeam = secondTeam == null ? firstTeam : null,
                IsFinished = secondTeam == null ? true : false,
                IsWinnersBracket = isWinnersBracket
            };

            newMatches.Add(match);
            matchNumber++;
        }

        return newMatches;
    }

    private async Task SaveMatches(Tournament tournament, IEnumerable<Match> matches)
    {
        foreach (var match in matches)
        {
            await _unitOfWork.MatchesRepository.AddAsync(match);

            if (match.FirstTeam != null)
            {
                match.FirstTeam.Matches.Add(match);
                _unitOfWork.TeamsRepository.Update(match.FirstTeam);
            }

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
}
