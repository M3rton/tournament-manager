using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Repositories;

public interface ITeamsRepository
{
    Task<Team?> GetTeamByNameAsync(string teamName);

    Task<Team?> GetTeamByPlayerAsync(Player player);

    Task SaveTeamAsync(Team team);
}
