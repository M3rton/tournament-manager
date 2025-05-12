using System.Collections.ObjectModel;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.Services;

internal class TeamsService : ITeamsService
{
    private readonly ITeamsRepository _teamsRepository;
    private readonly IPlayersRepository _playersRepository;

    public TeamsService(ITeamsRepository teamsRepository, IPlayersRepository playersRepository)
    {
        _teamsRepository = teamsRepository;
        _playersRepository = playersRepository;
    }

    public async Task CreateTeamAsync(string teamName, string tag, Player player)
    {
        if (await CanCreateTeamAsync(teamName))
        {
            var newTeam = new Team
            {
                Name = teamName,
                Tag = tag,
                TeamCaptain = player,
                Players = new ObservableCollection<Player> { player }
            };
            player.Team = newTeam;

            await _teamsRepository.SaveTeamAsync(newTeam);
        }
    }

    public async Task<bool> CanCreateTeamAsync(string teamName)
    {
        if (await _teamsRepository.GetTeamByNameAsync(teamName) != null)
        {
            return false;
        }
        return true;
    }

    public async Task JoinTeamAsync(Team team, string playerName)
    {
        Player? player = await _playersRepository.GetPlayerByName(playerName);

        if (player != null && player.Team == null && team.Players.Count < 5)
        {
            await _teamsRepository.AddPlayerToTeam(team, player);
        }
    }

    public async Task<Team?> GetTeamByIdAsync(int teamId)
    {
        return await _teamsRepository.GetTeamByIdAsync(teamId);
    }
}
