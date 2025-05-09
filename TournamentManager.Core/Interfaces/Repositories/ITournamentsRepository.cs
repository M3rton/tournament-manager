using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Repositories;

public interface ITournamentsRepository
{
    Tournament GetTournamentById(int id);
}
