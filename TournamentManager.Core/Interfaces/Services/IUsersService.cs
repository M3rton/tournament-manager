using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Services;

public interface IUsersService
{
    Task<User?> LoginAsync(string userName, string password);

    Task RegisterAsync(string userName, string password);

    Task<bool> CanRegisterAsync(string userName);

    Task LoadUserAccount(User user);

    Task LoadUserTeam(User user);

    Task LoadUserTournament(User user);
}
