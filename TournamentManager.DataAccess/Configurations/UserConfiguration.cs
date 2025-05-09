using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentManager.Core.Entities;

namespace TournamentManager.DataAccess.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(nameof(User.UserId));

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(p => p.Name)
            .IsUnique();

        builder.Property(p => p.HashedPassword)
            .IsRequired();
    }
}
