using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.DataAccess.Data;

namespace TournamentManager.DataAccess.Repositories;

internal class TournamentsRepository : ITournamentsRepository
{
    private readonly TournamentDbContext _db;

    public TournamentsRepository(TournamentDbContext db) 
    {
        _db = db;
    }

    public Tournament GetTournamentById(int id)
    {
        return _db.Set<Tournament>()
            .Where(t => t.TournamentId == id)
            .First();
    }
}
