namespace Cochief.Infrastructure.Persistence.Configurations;

using Cochief.Domain.Model;
using Cochief.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("players");

        builder.HasKey(player => player.Id);
        builder.Property(player => player.Id).ValueGeneratedNever();
        builder.Property(player => player.Name).HasMaxLength(100).IsRequired();
        builder.Property(player => player.Tag)
            .HasConversion(tag => tag.Value, value => Tag.Restore(value))
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(player => player.TownHallLevel).IsRequired();
        builder.HasIndex(player => player.Tag).IsUnique();

        builder
            .HasOne<Clan>()
            .WithMany()
            .HasForeignKey(player => player.ClanId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
