using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.Services;

internal class PlayersService : IPlayersService
{
    private readonly IPlayersRepository _playersRepository;
    private readonly ITeamsRepository _teamsRepository;

    public PlayersService(IPlayersRepository playersRepository, ITeamsRepository teamsRepository)
    {
        _playersRepository = playersRepository;
        _teamsRepository = teamsRepository;
    }

    public async Task<Player?> GetPlayerByIdAsync(int playerId)
    {
        return await _playersRepository.GetPlayerById(playerId);
    }

    public async Task LeaveTeamAsync(Player player)
    {
        if (player == null)
        {
            return;
        }

        Team? team = player.Team;
        if (team != null)
        {
            await _teamsRepository.RemovePlayer(team, player);
        }
    }
}
