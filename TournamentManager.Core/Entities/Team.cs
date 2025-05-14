using System.Collections.ObjectModel;

namespace TournamentManager.Core.Entities;

public class Team
{
    public int TeamId { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    public virtual Player? TeamCaptain { get; set; }
    public virtual ObservableCollection<Player> Players { get; set; }
    public virtual ObservableCollection<Tournament> Tournaments { get; set; }
    public virtual ObservableCollection<Match> Matches { get; set; }
}
