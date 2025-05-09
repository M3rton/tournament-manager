using Microsoft.EntityFrameworkCore;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.DataAccess.Data;

namespace TournamentManager.DataAccess.Repositories;

internal class TeamsRepository : ITeamsRepository
{
    private TournamentDbContext _db;

    public TeamsRepository(TournamentDbContext db)
    {
        _db = db;
    }

    public async Task<Team?> GetTeamByNameAsync(string teamName)
    {
        return await _db.Teams
            .Where(t => t.Name == teamName)
            .FirstOrDefaultAsync();
    }

    public async Task<Team?> GetTeamByPlayerAsync(Player player)
    {
        return await _db.Teams
            .Include(t => t.Players)
            .Where(t => t.Players.Contains(player))
            .FirstOrDefaultAsync();
    }

    public async Task SaveTeamAsync(Team team)
    {
        _db.Teams.Add(team);
        await _db.SaveChangesAsync();
    }
}
