using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task SaveAsync();
    IGenericRepository<Match> MatchesRepository { get; }
    IGenericRepository<Player> PlayersRepository { get; }
    IGenericRepository<Team> TeamsRepository { get; }
    IGenericRepository<Tournament> TournamentsRepository { get; }
    IGenericRepository<User> UsersRepository { get; }
}
