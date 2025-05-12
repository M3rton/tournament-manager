using TournamentManager.Core.Entities;
using TournamentManager.Core.Enums;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.Services;

internal class TournamentsService : ITournamentsService
{
    private readonly ITournamentsRepository _tournamentsRepository;
    private readonly ITeamsRepository _teamsRepository;
    private readonly IMatchesRepository _matchesRepository;

    public TournamentsService(ITournamentsRepository tournamentsRepository, ITeamsRepository teamsRepository, IMatchesRepository matchesRepository)
    {
        _tournamentsRepository = tournamentsRepository;
        _teamsRepository = teamsRepository;
        _matchesRepository = matchesRepository;
    }

    public async Task<Tournament?> GetTournamentByIdAsync(int tournamentId)
    {
        return await _tournamentsRepository.GetTournamentByIdAsync(tournamentId);
    }

    public async Task CreateTournamentAsync(string tournamentName, StrategyType? strategyType, int? maxTeams, string? description, Player player)
    {
        if (strategyType == null || maxTeams == null || ! await CanCreateTournamentAsync(tournamentName) || player.Tournament != null)
        {
            return;
        }

        var tournament = new Tournament
        {
            Name = tournamentName,
            Strategy = strategyType.Value,
            MaxTeams = maxTeams.Value,
            Description = description,
            Owner = player
        };
        player.Tournament = tournament;

        await _tournamentsRepository.SaveTournamentAsync(tournament);
    }

    public async Task<bool> CanCreateTournamentAsync(string tournamentName)
    {
        return await _tournamentsRepository.GetTournamentByName(tournamentName) == null;
    }

    public async Task AddTeamAsync(Tournament tournament, string teamName)
    {
        Team? team = await _teamsRepository.GetTeamByNameAsync(teamName);

        if (team != null)
        {
            if (tournament.Teams.Contains(team))
            {
                return;
            }

            await _tournamentsRepository.AddTeam(tournament, team);
        }
    }

    public async Task SaveWinnerAsync(Tournament tournament, Team team)
    {
        if (tournament != null)
        {
            _tournamentsRepository.SaveWinnerAsync(tournament, team);
        }
    }

    public async Task<IEnumerable<Match>> GenerateBracketAsync(Tournament tournament)
    {
        switch (tournament.Strategy)
        {
            case StrategyType.Spider:
                return GenerateSpiderBracketAsync(tournament);
            case StrategyType.Groups:
                return GenerateGroupsBracketAsync(tournament);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerable<Match> GenerateSpiderBracketAsync(Tournament tournament)
    {
        IEnumerable<Match> CreateMatches(int roundSize, List<Team> teams, int currentRound)
        {
            var newMatches = new List<Match>();

            for (int i = 0; i < roundSize; i++)
            {
                Match match = new Match
                {
                    Round = currentRound,
                    MatchId = tournament.Matches.Count + 1,
                    FirstTeam = teams[i],
                    SecondTeam = teams[roundSize * 2 - 1 - i] == null ? null : teams[roundSize * 2 - 1 - i],
                    WinnerTeam = teams[roundSize * 2 - 1 - i] == null ? teams[i] : null,
                    IsFinished = teams[roundSize * 2 - 1 - i] == null ? true : false
                };

                _matchesRepository.SaveMatchAsync(match);
                _tournamentsRepository.AddMatch(tournament, match);

                newMatches.Add(match);
            }

            return newMatches;
        }

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

            return CreateMatches(bracketSize / 2, tournament.Teams.ToList(), 1);
        }
        else
        {
            List<Team> currentTeams = new List<Team>();

            int currentRound = tournament.Matches.Last().Round + 1;

            foreach (Match match in tournament.Matches)
            {
                if (!match.IsFinished)
                {
                    return Enumerable.Empty<Match>();
                }

                if (match.Round == currentRound - 1)
                {
                    currentTeams.Add(match.WinnerTeam!);
                }
            }

            if (currentTeams.Count <= 1)
            {
                return Enumerable.Empty<Match>();
            }

            return CreateMatches(currentTeams.Count / 2, currentTeams, currentRound);
        }
    }

    private IEnumerable<Match> GenerateGroupsBracketAsync(Tournament tournament)
    {
        throw new NotImplementedException();
    }
}
