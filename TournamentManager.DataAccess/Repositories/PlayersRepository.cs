using Microsoft.EntityFrameworkCore;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.DataAccess.Data;

namespace TournamentManager.DataAccess.Repositories;

internal class PlayersRepository : IPlayersRepository
{
    private TournamentDbContext _db;

    public PlayersRepository(TournamentDbContext db)
    {
        _db = db;
    }
    public async Task<Player?> GetPlayerByName(string playerName)
    {
        return await _db.Set<Player>()
            .Where(p => p.Name == playerName)
            .FirstOrDefaultAsync();
    }
}
