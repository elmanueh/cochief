namespace Cochief.Infrastructure.Persistence.Configurations;

using Cochief.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MemberConfiguration : IEntityTypeConfiguration<MemberEntity>
{
    public void Configure(EntityTypeBuilder<MemberEntity> builder)
    {
        builder.ToTable("members");

        builder.HasKey(member => member.Id);
        builder.Property(member => member.Id).ValueGeneratedNever();
        builder.Property(member => member.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(member => member.PlayerId).IsUnique();

        builder
            .HasOne<PlayerEntity>()
            .WithOne()
            .HasForeignKey<MemberEntity>(member => member.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<ClanEntity>()
            .WithMany(clan => clan.Members)
            .HasForeignKey(member => member.ClanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
