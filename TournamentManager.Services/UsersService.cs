using Microsoft.AspNetCore.Identity;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.Services;

internal class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly PasswordHasher<string> passwordHasher;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
        passwordHasher = new PasswordHasher<string>();
    }

    public async Task<User?> LoginAsync(string userName, string password)
    {
        User? user = await _usersRepository.GetUserByNameAsync(userName);

        if (user == null || 
            passwordHasher.VerifyHashedPassword(userName, user.HashedPassword, password) != PasswordVerificationResult.Success)
        {
            return null;
        }

        return user;
    }

    public async Task RegisterAsync(string userName, string password)
    {
        if (await CanRegisterAsync(userName))
        {
            string hashedPassword = passwordHasher.HashPassword(userName, password);

            Player player = new Player { Name = userName };
            User user = new User { Name = userName, HashedPassword = hashedPassword, Account = player };

            await _usersRepository.SaveUserAsync(user);
        }

    }

    public async Task<bool> CanRegisterAsync(string userName)
    {
        if (await _usersRepository.GetUserByNameAsync(userName) != null)
        {
            return false;
        }
        return true;
    }

    public async Task LoadUserAccount(User user)
    {
        await _usersRepository.LoadUserAccount(user);
    }

    public async Task LoadUserTeam(User user)
    {
        await _usersRepository.LoadUserTeam(user);
    }

    public async Task LoadUserTournament(User user)
    {
        await _usersRepository.LoadUserTournament(user);
    }
}
