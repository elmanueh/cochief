namespace Cochief.Infrastructure.Persistence.Configurations;

using Cochief.Domain.Model;
using Cochief.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class ClanConfiguration : IEntityTypeConfiguration<Clan>
{
    public void Configure(EntityTypeBuilder<Clan> builder)
    {
        builder.ToTable("clans");

        builder.HasKey(clan => clan.Id);
        builder.Property(clan => clan.Id)
            .ValueGeneratedNever();
        builder.Property(clan => clan.Name)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(clan => clan.Tag)
            .HasConversion(tag => tag.Value, value => Tag.Restore(value))
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(clan => clan.Tag).IsUnique();

        builder.Navigation(clan => clan.Members)
            .HasField("_members")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
