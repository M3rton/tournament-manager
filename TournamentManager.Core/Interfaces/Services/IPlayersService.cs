using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Services;

public interface IPlayersService
{
    Task<Player?> GetPlayersByTeam(Team team);
}
