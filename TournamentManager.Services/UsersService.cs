using Microsoft.AspNetCore.Identity;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.Services;

internal class UsersService : IUsersService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PasswordHasher<string> passwordHasher;

    public UsersService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        passwordHasher = new PasswordHasher<string>();
    }

    public async Task<User?> LoginAsync(string userName, string password)
    {
        User? user = (await _unitOfWork.UsersRepository.GetAsync(u => u.Name == userName)).FirstOrDefault();

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

            await _unitOfWork.PlayersRepository.AddAsync(player);
            await _unitOfWork.UsersRepository.AddAsync(user);

            await _unitOfWork.SaveAsync();
        }
    }

    public async Task<bool> CanRegisterAsync(string userName)
    {
        return (await _unitOfWork.UsersRepository.GetAsync(u => u.Name.ToLower() == userName.ToLower())).FirstOrDefault() == null;
    }
}
