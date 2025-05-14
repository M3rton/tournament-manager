using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Events;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.ViewModels.ViewModels;

public partial class CreateTeamViewModel : ObservableObject
{
    public Player? Player { get; set; }

    private readonly ITeamsService _teamsService;

    private readonly PopUpMessageEvent _popUpMessageEvent;
    private readonly ChangeViewModelEvent _changeViewModelEvent;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateTeamCommand))]
    private string? _teamName;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateTeamCommand))]
    private string? _tag;

    public CreateTeamViewModel(ITeamsService teamsService, IEventAggregator eventAggregator)
    {
        _teamsService = teamsService;

        _popUpMessageEvent = eventAggregator.GetEvent<PopUpMessageEvent>();
        _changeViewModelEvent = eventAggregator.GetEvent<ChangeViewModelEvent>();
    }

    [RelayCommand(CanExecute = nameof(CanCreateTeam))]
    private async Task CreateTeam()
    {
        if (Player == null || Player.Team != null)
        {
            return;
        }

        if (TeamName == null || Tag == null)
        {
            return;
        }

        string message;
        bool teamCreated = false;

        if (await _teamsService.CanCreateTeamAsync(TeamName))
        {
            await _teamsService.CreateTeamAsync(TeamName, Tag, Player);
            message = "Successfully created new team.";

            teamCreated = true;
        }
        else
        {
            message = "Failed to create a team. This team name is already used.";
        }

        _popUpMessageEvent.Publish(new PopUpMessagePayload
        {
            Sender = this,
            Message = message
        });

        if (teamCreated)
        {
            _changeViewModelEvent.Publish(new ChangeViewModelPayload
            {
                Sender = this,
                ViewModelName = nameof(MyTeamViewModel)
            });
        }
    }

    private bool CanCreateTeam()
    {
        return !string.IsNullOrEmpty(TeamName) && !string.IsNullOrEmpty(Tag);
    }
}
