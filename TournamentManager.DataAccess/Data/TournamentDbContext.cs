using Microsoft.EntityFrameworkCore;
using TournamentManager.Core.Entities;
using TournamentManager.DataAccess.Configurations;

namespace TournamentManager.DataAccess.Data;

internal class TournamentDbContext : DbContext
{
    public DbSet<Tournament> Tournaments { get; set; }

    public DbSet<Team> Teams { get; set; }

    public DbSet<Player> Players { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Match> Matches { get; set; }

    public TournamentDbContext(DbContextOptions<TournamentDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TournamentConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new PlayerConfiguration());
        modelBuilder.ApplyConfiguration(new MatchConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
