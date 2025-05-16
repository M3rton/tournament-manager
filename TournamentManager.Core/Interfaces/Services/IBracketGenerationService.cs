using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Interfaces.Services;

public interface IBracketGenerationService
{
    Task<IEnumerable<Match>> GenerateBracketAsync(Tournament tournament);
}
