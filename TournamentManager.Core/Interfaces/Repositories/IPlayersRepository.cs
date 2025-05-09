using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Repositories;

public interface IPlayersRepository
{
    Task<Player?> GetPlayerByName(string playerName);
}
