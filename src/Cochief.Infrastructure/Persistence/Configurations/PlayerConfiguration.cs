namespace Cochief.Infrastructure.Persistence.Configurations;

using Cochief.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class PlayerConfiguration : IEntityTypeConfiguration<PlayerEntity>
{
    public void Configure(EntityTypeBuilder<PlayerEntity> builder)
    {
        builder.ToTable("players");

        builder.HasKey(player => player.Id);
        builder.Property(player => player.Id).ValueGeneratedNever();
        builder.Property(player => player.Name).HasMaxLength(100).IsRequired();
        builder.Property(player => player.Tag)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(player => player.TownHallLevel).IsRequired();
        builder.Property(player => player.ClanTag)
            .HasMaxLength(20);
        builder.HasIndex(player => player.Tag).IsUnique();
        builder.HasIndex(player => player.ClanTag);
    }
}
