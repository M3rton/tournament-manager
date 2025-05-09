namespace TournamentManager.Core.Entities;

public class Team
{
    public int TeamId { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    public Player TeamCaptain { get; set; }
    public List<Player> Players { get; set; }
    public List<Tournament> Tournaments { get; set; }
    public List<Match> Matches { get; set; }
}
