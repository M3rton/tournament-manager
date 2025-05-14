using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.DataAccess.Data;

namespace TournamentManager.DataAccess.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly TournamentDbContext _dbContext;
    private IGenericRepository<Match>? _matchesRepository;
    private IGenericRepository<Player>? _playersRepository;
    private IGenericRepository<Team>? _teamsRepository;
    private IGenericRepository<Tournament>? _tournamentsRepository;
    private IGenericRepository<User>? _usersRepository;

    public UnitOfWork(TournamentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public IGenericRepository<Match> MatchesRepository
    {
        get
        {
            if (_matchesRepository == null)
            {
                _matchesRepository = new GenericRepository<Match>(_dbContext.Matches);
            }

            return _matchesRepository;
        }
    }

    public IGenericRepository<Player> PlayersRepository
    {
        get
        {
            if (_playersRepository == null)
            {
                _playersRepository = new GenericRepository<Player>(_dbContext.Players);
            }

            return _playersRepository;
        }
    }

    public IGenericRepository<Team> TeamsRepository
    {
        get
        {
            if (_teamsRepository == null)
            {
                _teamsRepository = new GenericRepository<Team>(_dbContext.Teams);
            }

            return _teamsRepository;
        }
    }

    public IGenericRepository<Tournament> TournamentsRepository
    {
        get
        {
            if (_tournamentsRepository == null)
            {
                _tournamentsRepository = new GenericRepository<Tournament>(_dbContext.Tournaments);
            }

            return _tournamentsRepository;
        }
    }

    public IGenericRepository<User> UsersRepository
    {
        get
        {
            if (_usersRepository == null)
            {
                _usersRepository = new GenericRepository<User>(_dbContext.Users);
            }

            return _usersRepository;
        }
    }

}
