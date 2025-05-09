using TournamentManager.Core.Enums;

namespace TournamentManager.Core.Entities;
public class Tournament
{
    public int TournamentId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateStart { get; set; }
    public StrategyType Strategy {  get; set; }
    public int MaxTeams { get; set; }
    public List<Team> Teams { get; set; }
    public List<Match> Matches { get; set; }
}
