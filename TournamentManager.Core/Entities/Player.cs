namespace TournamentManager.Core.Entities;

public class Player
{
    public int PlayerId { get; set; }
    public string Name { get; set; }
    public virtual Team? Team { get; set; }
    public virtual Tournament? Tournament { get; set; }
    public string? MainRole { get; set; }
    public string? SecondaryRole { get; set; }
}
