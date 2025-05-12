using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.DataAccess.Data;

namespace TournamentManager.DataAccess.Repositories;

internal class MatchesRepository : IMatchesRepository
{
    private TournamentDbContext _db;

    public MatchesRepository(TournamentDbContext db)
    {
        _db = db;
    }

    public async Task SaveMatchAsync(Match match)
    {
        _db.Matches.Add(match);
        await _db.SaveChangesAsync();
    }
}
