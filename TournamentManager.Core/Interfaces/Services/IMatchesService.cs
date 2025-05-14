using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Services;

public interface IMatchesService
{
    Task SaveWinnerAsync(Match match, Team team);
}
