using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentManager.Core.Entities;

namespace TournamentManager.DataAccess.Configurations;

internal class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(nameof(Match.MatchId));

        builder.HasOne(m => m.FirstTeam)
            .WithMany()
            .HasForeignKey("FirstTeamId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.SecondTeam)
            .WithMany()
            .HasForeignKey("SecondTeamId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.WinnerTeam)
            .WithMany()
            .HasForeignKey("WinnerTeamId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.FirstTeamWins)
            .HasDefaultValue(0);

        builder.Property(m => m.SecondTeamWins)
            .HasDefaultValue(0);

        builder.Property(m => m.IsFinished)
            .HasDefaultValue(false);
    }
}
