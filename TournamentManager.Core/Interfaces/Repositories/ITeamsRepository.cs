using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Repositories;

public interface ITeamsRepository
{
    Task<Team?> GetTeamByNameAsync(string teamName);

    Task<Team?> GetTeamByIdAsync(int teamId);

    Task AddPlayerToTeam(Team team, Player player);

    Task RemovePlayer(Team team, Player player);

    Task SaveTeamAsync(Team team);
}
