namespace Cochief.Infrastructure.Persistence.Configurations;

using Cochief.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
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
            .HasOne<Player>()
            .WithOne()
            .HasForeignKey<Member>(member => member.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<Clan>()
            .WithMany(clan => clan.Members)
            .HasForeignKey(member => member.ClanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
