namespace TournamentManager.Core.Entities;

public class User
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string HashedPassword { get; set; }
    public virtual Player Account { get; set; }
}
