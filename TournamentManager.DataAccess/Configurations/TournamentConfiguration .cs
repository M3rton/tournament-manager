using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentManager.Core.Entities;

namespace TournamentManager.DataAccess.Configurations;

internal class TournamentConfiguration : IEntityTypeConfiguration<Tournament>
{
    public void Configure(EntityTypeBuilder<Tournament> builder)
    {
        builder.HasKey(nameof(Tournament.TournamentId));

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(p => p.Description)
            .HasMaxLength(255);

        builder.Property(p => p.DateCreated)
            .IsRequired();

        builder.Property(p => p.DateStart)
            .IsRequired();

        builder.Property(p => p.Strategy)
            .IsRequired();

        builder.Property(p => p.MaxTeams)
            .IsRequired();
    }
}
