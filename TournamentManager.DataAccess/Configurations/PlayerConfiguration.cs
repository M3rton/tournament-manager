using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentManager.Core.Entities;

namespace TournamentManager.DataAccess.Configurations;

internal class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(nameof(Player.PlayerId));

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(p => p.Team)
            .WithMany(t => t.Players)
            .HasForeignKey("TeamId")
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
