namespace TournamentManager.Core.Entities;

public class Match
{
    public int MatchId { get; set; }
    public int Round { get; set; }
    public int MatchNumber { get; set; }
    public virtual Tournament? Tournament { get; set; }
    public virtual Team FirstTeam { get; set; }
    public virtual Team? SecondTeam { get; set; }
    public virtual Team? WinnerTeam { get; set; }
    public int FirstTeamWins { get; set; }
    public int SecondTeamWins { get; set; }
    public bool IsFinished { get; set; }
}
