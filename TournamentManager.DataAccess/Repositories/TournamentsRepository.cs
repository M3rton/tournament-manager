using Microsoft.EntityFrameworkCore;
using System.Numerics;
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

    public async Task<Tournament?> GetTournamentByIdAsync(int tournamentId)
    {
        return await _db.Tournaments
            .Include(t => t.Teams)
            .Include(t => t.Owner)
            .Include(t => t.Matches)
                .ThenInclude(m => m.FirstTeam)
            .Include(t => t.Matches)
                .ThenInclude(m => m.SecondTeam)
            .Include(t => t.Matches)
                .ThenInclude(m => m.WinnerTeam)
            .Where(t => t.TournamentId == tournamentId)
            .FirstOrDefaultAsync();
    }

    public async Task<Tournament?> GetTournamentByName(string tournamentName)
    {
        return await _db.Tournaments
            .Include(t => t.Teams)
            .Include(t => t.Owner)
            .Include(t => t.Matches)
                .ThenInclude(m => m.FirstTeam)
            .Include(t => t.Matches)
                .ThenInclude(m => m.SecondTeam)
            .Include(t => t.Matches)
                .ThenInclude(m => m.WinnerTeam)
            .Where(t => t.Name == tournamentName)
            .FirstOrDefaultAsync();
    }

    public async Task SaveTournamentAsync(Tournament tournament)
    {
        _db.Tournaments.Add(tournament);
        await _db.SaveChangesAsync();
    }

    public async Task AddTeam(Tournament tournament, Team team)
    {
        _db.Tournaments.Attach(tournament);
        _db.Teams.Attach(team);

        tournament.Teams.Add(team);
        team.Tournaments.Add(tournament);

        await _db.SaveChangesAsync();
    }

    public async Task AddMatch(Tournament tournament, Match match)
    {
        _db.Tournaments.Attach(tournament);
        _db.Matches.Attach(match);

        tournament.Matches.Add(match);
        match.Tournament = tournament;

        await _db.SaveChangesAsync();
    }

    public async Task SaveWinnerAsync(Tournament tournament, Team team)
    {
        _db.Tournaments.Attach(tournament);

        tournament.Winner = team;

        await _db.SaveChangesAsync();
    }
}
