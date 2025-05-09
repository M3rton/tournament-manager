using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.Services;

internal class TeamsService : ITeamsService
{
    private readonly ITeamsRepository _teamsRepository;

    public TeamsService(ITeamsRepository teamsRepository)
    {
        _teamsRepository = teamsRepository;
    }

    public async Task CreateTeamAsync(string teamName, string tag, Player player)
    {
        if (await CanCreateTeamAsync(teamName, player))
        {
            var newTeam = new Team
                {
                    Name = teamName,
                    Tag = tag,
                    TeamCaptain = player,
                    Players = new List<Player> { player }
                };

            player.Team = newTeam;
            await _teamsRepository.SaveTeamAsync(newTeam);
        }
    }

    public async Task<bool> CanCreateTeamAsync(string teamName, Player player)
    {
        if (await _teamsRepository.GetTeamByNameAsync(teamName) != null ||
            await _teamsRepository.GetTeamByPlayerAsync(player) != null)
        {
            return false;
        }
        return true;
    }

    public Task<bool> JoinTeamAsync()
    {
        throw new NotImplementedException();
    }

    public Task LeaveTeamAsync()
    {
        throw new NotImplementedException();
    }
}
