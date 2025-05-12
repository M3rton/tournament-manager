namespace TournamentManager.Core.Entities;

public class Match
{
    public int MatchId { get; set; }
    public int Round { get; set; }
    public int MatchNumber { get; set; }
    public Tournament? Tournament { get; set; }
    public Team FirstTeam { get; set; }
    public Team? SecondTeam { get; set; }
    public Team? WinnerTeam { get; set; }
    public bool IsFinished { get; set; }
    public int FirstTeamWins { get; set; }
    public int SecondTeamWins { get; set; }
}
