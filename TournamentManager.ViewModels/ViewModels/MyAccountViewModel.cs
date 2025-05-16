using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Enums;

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

    public MyAccountViewModel()
    {
    }

    [RelayCommand (CanExecute = nameof(CanSaveChanges))]
    private void SaveChanges()
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
