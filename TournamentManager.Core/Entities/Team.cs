using System.Collections.ObjectModel;

namespace TournamentManager.Core.Entities;

public class Team
{
    public int TeamId { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    public Player? TeamCaptain { get; set; }
    public ObservableCollection<Player> Players { get; set; }
    public ObservableCollection<Tournament> Tournaments { get; set; }
    public ObservableCollection<Match> Matches { get; set; }
}
