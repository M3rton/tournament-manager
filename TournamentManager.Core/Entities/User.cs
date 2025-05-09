namespace TournamentManager.Core.Entities;

public class User
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string HashedPassword { get; set; }
    public Player Account { get; set; }
}
