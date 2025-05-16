using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Services;

public interface IPlayersService
{
    Task UpdateInformationsAsync(Player player);
    Task LeaveTeamAsync(Player player);
}
