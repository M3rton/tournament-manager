using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.Services;

internal class MatchesService : IMatchesService
{
    private readonly IUnitOfWork _unitOfWork;

    public MatchesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task SaveWinnerAsync(Match match, Team team)
    {
        if (match.FirstTeam == team || match.SecondTeam == team)
        {
            match.WinnerTeam = team;
            match.IsFinished = true;

            _unitOfWork.MatchesRepository.Update(match);

            await _unitOfWork.SaveAsync();
        }
    }
}
