using TournamentManager.Core.Enums;

namespace TournamentManager.Core.Entities;

public class Player
{
    public int PlayerId { get; set; }
    public string Name { get; set; }
    public virtual Team? Team { get; set; }
    public virtual Tournament? Tournament { get; set; }
    public Role? MainRole { get; set; }
    public Role? SecondaryRole { get; set; }
}
