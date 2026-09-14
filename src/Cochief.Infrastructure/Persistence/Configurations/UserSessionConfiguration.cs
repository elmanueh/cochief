namespace Cochief.Infrastructure.Persistence.Configurations;

using Cochief.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("user_sessions");

        builder.HasKey(session => session.Id);
        builder.Property(session => session.Id).ValueGeneratedNever();
        builder.Property(session => session.UserId).IsRequired();
        builder.Property(session => session.RefreshTokenHash).HasMaxLength(64).IsRequired();
        builder.Property(session => session.CreatedAt).IsRequired();
        builder.Property(session => session.ExpiresAt).IsRequired();
        builder.Property(session => session.RevokedAt);

        builder.HasIndex(session => session.RefreshTokenHash).IsUnique();
        builder.HasIndex(session => session.UserId);

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
