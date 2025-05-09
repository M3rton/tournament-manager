using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TournamentManager.Core.Interfaces.Navigation;
using TournamentManager.Core.Interfaces.Services;
using TournamentManager.ViewModels.Interfaces;

namespace TournamentManager.ViewModels.ViewModels;

public partial class CreateTeamViewModel : UserViewModel
{
    private readonly IViewModelFactory<MyTeamViewModel> _myTeam;
    private readonly ITeamsService _teamsService;
    private readonly IWindowManager _windowManager;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateTeamCommand))]
    private string? _teamName;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateTeamCommand))]
    private string? _tag;

    public CreateTeamViewModel(
        IViewModelFactory<MyTeamViewModel> myTeam,
        ITeamsService teamsService,
        IWindowManager windowManager)
    {
        _myTeam = myTeam;
        _teamsService = teamsService;
        _windowManager = windowManager;
    }

    [RelayCommand(CanExecute = nameof(CanCreateTeam))]
    private async Task CreateTeam()
    {
        string message;

        if (await _teamsService.CanCreateTeamAsync(TeamName!, User!.Account))
        {
            await _teamsService.CreateTeamAsync(TeamName!, Tag!, User!.Account);
            message = "Successfully created new team.";
            OnChangeViewModel(_myTeam.Create());
        }
        else
        {
            message = "Failed to create a team. This team name is already used.";
        }
        TeamName = "";
        Tag = "";

        _windowManager.ShowWindow<PopUpWindowViewModel>(x => x.Message = message);
    }

    private bool CanCreateTeam()
    {
        return !string.IsNullOrEmpty(TeamName) && !string.IsNullOrEmpty(Tag);
    }
}
