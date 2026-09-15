namespace Cochief.Infrastructure.Persistence.Configurations;

using Cochief.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class ClanConfiguration : IEntityTypeConfiguration<ClanEntity>
{
    public void Configure(EntityTypeBuilder<ClanEntity> builder)
    {
        builder.ToTable("clans");

        builder.HasKey(clan => clan.Id);
        builder.Property(clan => clan.Id)
            .ValueGeneratedNever();
        builder.Property(clan => clan.Name)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(clan => clan.Tag)
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(clan => clan.Tag).IsUnique();
    }
}
