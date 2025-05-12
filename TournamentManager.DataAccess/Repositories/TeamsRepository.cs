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
            .Include(t => t.Players)
            .Include(t => t.Tournaments)
            .Where(t => t.Name == teamName)
            .FirstOrDefaultAsync();
    }

    public async Task<Team?> GetTeamByIdAsync(int teamId)
    {
        return await _db.Teams
            .Include(t => t.Players)
            .Include(t => t.Tournaments)
            .Where(t => t.TeamId == teamId)
            .FirstOrDefaultAsync();
    }

    public async Task SaveTeamAsync(Team team)
    {
        _db.Teams.Add(team);
        await _db.SaveChangesAsync();
    }

    public async Task RemovePlayer(Team team, Player player)
    {
        if (team.Players.Contains(player))
        {
            _db.Players.Attach(player);

            player.Team = null;
            team.Players.Remove(player);
            if (player == team.TeamCaptain && team.Players.Count > 0)
            {
                team.TeamCaptain = team.Players.First();
            }
            else
            {
                team.TeamCaptain = null;
            }

            _db.Update(team);
            await _db.SaveChangesAsync();
        }
    }

    public async Task AddPlayerToTeam(Team team, Player player)
    {
        _db.Players.Attach(player);
        _db.Teams.Attach(team);

        player.Team = team;
        team.Players.Add(player);

        await _db.SaveChangesAsync();
    }
}
