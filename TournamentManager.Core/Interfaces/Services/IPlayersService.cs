using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Services;

public interface IPlayersService
{
    Task<Player?> GetPlayerByIdAsync(int playerId);

    Task LeaveTeamAsync(Player player);
}
