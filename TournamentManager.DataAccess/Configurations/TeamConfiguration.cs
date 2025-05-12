using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentManager.Core.Entities;

namespace TournamentManager.DataAccess.Configurations;

internal class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(nameof(Team.TeamId));

        builder.HasMany(t => t.Matches)
            .WithOne()
            .HasForeignKey("FirstTeamId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.TeamCaptain)
            .WithOne()
            .HasForeignKey<Team>("TeamCaptainId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Tag)
            .IsRequired()
            .HasMaxLength(4);
    }
}
