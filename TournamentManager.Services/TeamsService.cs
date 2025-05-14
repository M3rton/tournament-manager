using System.Collections.ObjectModel;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.Services;

internal class TeamsService : ITeamsService
{
    private readonly IUnitOfWork _unitOfWork;

    public TeamsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateTeamAsync(string teamName, string tag, Player player)
    {
        if (player.Team != null)
        {
            return;
        }

        if (await CanCreateTeamAsync(teamName))
        {
            var newTeam = new Team
            {
                Name = teamName,
                Tag = tag,
                TeamCaptain = player,
                Players = new ObservableCollection<Player> { player },
                Tournaments = new ObservableCollection<Tournament>(),
                Matches = new ObservableCollection<Match>()
            };
            player.Team = newTeam;

            await _unitOfWork.TeamsRepository.AddAsync(newTeam);
            _unitOfWork.PlayersRepository.Update(player);

            await _unitOfWork.SaveAsync();
        }
    }

    public async Task<bool> CanCreateTeamAsync(string teamName)
    {
        return (await _unitOfWork.TeamsRepository.GetAsync(t => t.Name == teamName)).FirstOrDefault() == null;
    }

    public async Task JoinTeamAsync(Team team, string playerName)
    {
        Player? player = (await _unitOfWork.PlayersRepository.GetAsync(p => p.Name == playerName)).FirstOrDefault();

        if (player != null && player.Team == null && team.Players.Count < 5)
        {
            player.Team = team;
            team.Players.Add(player);

            _unitOfWork.PlayersRepository.Update(player);
            _unitOfWork.TeamsRepository.Update(team);

            await _unitOfWork.SaveAsync();
        }
    }
}
