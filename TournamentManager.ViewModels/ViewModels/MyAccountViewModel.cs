using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Enums;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.ViewModels.ViewModels;

public partial class MyAccountViewModel : ObservableObject
{
    public Player? Player { get; set; }

    public ObservableCollection<Role> Roles { get; set; } =
        [.. Enum.GetValues(typeof(Role)).Cast<Role>()];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveChangesCommand))]
    private Role? _selectedMainRole;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveChangesCommand))]
    private Role? _selectedSecondaryRole;

    private readonly IPlayersService _playersService;

    public MyAccountViewModel(IPlayersService playersService)
    {
        _playersService = playersService;
    }

    [RelayCommand (CanExecute = nameof(CanSaveChanges))]
    private async Task SaveChanges()
    {
        if (Player == null)
        {
            return;
        }

        if (SelectedMainRole == null || SelectedSecondaryRole == null)
        {
            return;
        }

        Player.MainRole = (Role) SelectedMainRole;
        Player.SecondaryRole = (Role) SelectedSecondaryRole;

        await _playersService.UpdateInformationsAsync(Player);
    }

    private bool CanSaveChanges()
    {
        if (SelectedMainRole == null || SelectedSecondaryRole == null)
        {
            return false;
        }

        return true;
    }
}
