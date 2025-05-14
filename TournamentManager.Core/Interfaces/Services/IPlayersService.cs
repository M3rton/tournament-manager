using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Services;

public interface IPlayersService
{
    Task LeaveTeamAsync(Player player);
}
