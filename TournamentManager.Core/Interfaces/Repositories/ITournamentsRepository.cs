using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Repositories;

public interface ITournamentsRepository
{
    Task<Tournament?> GetTournamentByName(string tournamentName);

    Task<Tournament?> GetTournamentByIdAsync(int tournamentId);

    Task SaveTournamentAsync(Tournament tournament);

    Task AddTeam(Tournament tournament, Team team);

    Task AddMatch(Tournament tournament, Match match);
}
