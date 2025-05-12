using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentManager.Core.Entities;

namespace TournamentManager.DataAccess.Configurations;

internal class TournamentConfiguration : IEntityTypeConfiguration<Tournament>
{
    public void Configure(EntityTypeBuilder<Tournament> builder)
    {
        builder.HasKey(nameof(Tournament.TournamentId));

        builder.HasOne(t => t.Owner)
            .WithOne(p => p.Tournament)
            .HasForeignKey<Tournament>("OwnerId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Teams)
            .WithMany(tm => tm.Tournaments)
            .UsingEntity(j => j.ToTable("TournamentTeams"));

        builder.HasOne(t => t.Winner)
            .WithOne()
            .HasForeignKey<Team>("TeamWinnerId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Matches)
            .WithOne(m => m.Tournament)
            .HasForeignKey("MatchId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(p => p.Description)
            .HasMaxLength(255);

        builder.Property(p => p.Strategy)
            .IsRequired();

        builder.Property(p => p.MaxTeams)
            .IsRequired();
    }
}
