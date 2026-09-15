namespace Cochief.Infrastructure.Persistence.Configurations;

using Cochief.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedNever();
        builder.Property(user => user.Name).HasMaxLength(100).IsRequired();
        builder.Property(user => user.Email)
            .HasMaxLength(320)
            .IsRequired();
        builder.Property(user => user.PasswordHash).HasMaxLength(512).IsRequired();
        builder.HasIndex(user => user.Email).IsUnique();

        builder
            .HasOne(user => user.Player)
            .WithOne()
            .HasForeignKey<UserEntity>(user => user.PlayerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
