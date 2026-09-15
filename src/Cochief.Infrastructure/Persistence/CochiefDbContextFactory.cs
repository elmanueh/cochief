namespace Cochief.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

internal sealed class CochiefDbContextFactory : IDesignTimeDbContextFactory<CochiefDbContext>
{
    public CochiefDbContext CreateDbContext(string[] args)
    {
        string connectionString = GetConnectionString(args)
            ?? "Host=localhost;Database=cochief;Username=cochief;Password=design-time-only";

        DbContextOptionsBuilder<CochiefDbContext> options = new DbContextOptionsBuilder<CochiefDbContext>();
        options.UseNpgsql(connectionString);

        return new CochiefDbContext(options.Options);
    }

    private static string? GetConnectionString(string[] args)
    {
        int connectionIndex = Array.IndexOf(args, "--connection");
        if (connectionIndex >= 0 && connectionIndex + 1 < args.Length)
        {
            return args[connectionIndex + 1];
        }

        return Environment.GetEnvironmentVariable("ConnectionStrings__PostgreSql");
    }
}
