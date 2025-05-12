using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Repositories;

public interface IUsersRepository
{
    Task<User?> GetUserByNameAsync(string username);

    Task SaveUserAsync(User user);

    Task LoadUserAccount(User user);

    Task LoadUserTeam(User user);

    Task LoadUserTournament(User user);
}
