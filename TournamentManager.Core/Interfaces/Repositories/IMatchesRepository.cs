using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Repositories;

public interface IMatchesRepository
{
    Task SaveMatchAsync(Match match);
}
