using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Services;

public interface ITeamsService
{
    Task CreateTeamAsync(string teamName, string tag, Player player);
    Task<bool> CanCreateTeamAsync(string teamName, Player player);
    Task<bool> JoinTeamAsync();
    Task LeaveTeamAsync();
}
