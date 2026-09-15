namespace Cochief.Infrastructure.Persistence;

using Cochief.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class CochiefDbContext(DbContextOptions<CochiefDbContext> options) : DbContext(options)
{
    internal DbSet<UserEntity> Users => Set<UserEntity>();
    internal DbSet<UserSessionEntity> UserSessions => Set<UserSessionEntity>();
    internal DbSet<PlayerEntity> Players => Set<PlayerEntity>();
    internal DbSet<ClanEntity> Clans => Set<ClanEntity>();
    internal DbSet<MemberEntity> Members => Set<MemberEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CochiefDbContext).Assembly);
    }
}
