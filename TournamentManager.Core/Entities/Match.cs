namespace TournamentManager.Core.Entities;

public class Match
{
    public int MatchId { get; set; }
    public Team FirstTeam { get; set; }
    public Team SecondTeam { get; set; }
}
