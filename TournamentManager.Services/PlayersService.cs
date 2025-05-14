using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.Services;

internal class PlayersService : IPlayersService
{
    private readonly IUnitOfWork _unitOfWork;

    public PlayersService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task LeaveTeamAsync(Player player)
    {
        Team? team = player.Team;
        if (team != null)
        {
            team.Players.Remove(player);

            if (team.TeamCaptain == player)
            {
                team.TeamCaptain = team.Players.FirstOrDefault();
            }

            _unitOfWork.PlayersRepository.Update(player);
            _unitOfWork.TeamsRepository.Update(team);

            await _unitOfWork.SaveAsync();
        }
    }
}
