using Microsoft.EntityFrameworkCore;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.DataAccess.Data;

namespace TournamentManager.DataAccess.Repositories;

internal class UsersRepository : IUsersRepository
{
    private TournamentDbContext _db;

    public UsersRepository(TournamentDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetUserByNameAsync(string username)
    {
        return await _db.Users
            .Where(u => u.Name == username)
            .FirstOrDefaultAsync();
    }

    public async Task SaveUserAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task LoadUserAccount(User user)
    {
        await _db.Entry(user)
            .Reference(u => u.Account)
            .LoadAsync();
    }

    public async Task LoadUserTeam(User user)
    {
        await _db.Entry(user)
            .Reference(u => u.Account)
            .Query()
            .Include(a => a.Team)
            .LoadAsync();

        if (user.Account.Team != null)
        {
            await _db.Entry(user.Account.Team)
                .Collection(t => t.Players)
                .LoadAsync();
        }
    }

    public async Task LoadUserTournament(User user)
    {
        await _db.Entry(user)
            .Reference(u => u.Account)
            .Query()
            .Include(a => a.Tournament)
            .LoadAsync();

        if (user.Account.Tournament != null)
        {
            await _db.Entry(user.Account.Tournament)
                .Collection(t => t.Teams)
                .LoadAsync();

            await _db.Entry(user.Account.Tournament)
                .Collection(t => t.Matches)
                .LoadAsync();

            await _db.Entry(user.Account.Tournament)
                .Reference(t => t.Winner)
                .LoadAsync();
        }
    }
}
