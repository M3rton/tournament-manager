using CommunityToolkit.Mvvm.ComponentModel;
using TournamentManager.Core.Entities;

namespace TournamentManager.ViewModels.ViewModels;

public class MyAccountViewModel : ObservableObject
{
    public User? User { get; set; }
}
