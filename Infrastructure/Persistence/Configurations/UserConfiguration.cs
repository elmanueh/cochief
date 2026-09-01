namespace Cochief.Infrastructure.Persistence.Configurations;

using Cochief.Domain.Model;
using Cochief.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedNever();
        builder.Property(user => user.Name).HasMaxLength(100).IsRequired();
        builder.Property(user => user.Email)
            .HasConversion(email => email.Value, value => Email.Restore(value))
            .HasMaxLength(320)
            .IsRequired();
        builder.Property(user => user.PasswordHash).HasMaxLength(512).IsRequired();
        builder.HasIndex(user => user.Email).IsUnique();

        builder
            .HasOne(user => user.Player)
            .WithOne()
            .HasForeignKey<User>("PlayerId")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
