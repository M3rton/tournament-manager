using System.Collections.ObjectModel;
using TournamentManager.Core.Enums;

namespace TournamentManager.Core.Entities;
public class Tournament
{
    public int TournamentId { get; set; }
    public string Name { get; set; }
    public virtual Player Owner { get; set; }
    public string? Description { get; set; }
    public StrategyType Strategy {  get; set; }
    public int MaxTeams { get; set; }
    public virtual Team? Winner { get; set; }
    public virtual ObservableCollection<Team> Teams { get; set; }
    public virtual ObservableCollection<Match> Matches { get; set; }
}
