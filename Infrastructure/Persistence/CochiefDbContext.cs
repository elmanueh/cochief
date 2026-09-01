namespace Cochief.Infrastructure.Persistence;

using Cochief.Domain.Model;
using Microsoft.EntityFrameworkCore;

public sealed class CochiefDbContext(DbContextOptions<CochiefDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Clan> Clans => Set<Clan>();
    public DbSet<Member> Members => Set<Member>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CochiefDbContext).Assembly);
    }
}
