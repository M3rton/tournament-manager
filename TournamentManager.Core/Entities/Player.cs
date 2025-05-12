namespace TournamentManager.Core.Entities;

public class Player
{
    public int PlayerId { get; set; }
    public string Name { get; set; }
    public Team? Team { get; set; }
    public Tournament? Tournament { get; set; }
    public string? MainRole { get; set; }
    public string? SecondaryRole { get; set; }
}
