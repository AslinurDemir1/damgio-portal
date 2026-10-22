using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Damgio.Infrastructure.Data;

/// <summary>
/// Used by dotnet-ef tools at design-time (e.g. dotnet ef migrations add).
/// Allows generating migrations without requiring runtime secrets or active application host.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Default design-time connection string for migrations generation
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=damgio;Username=damgio;Password=local-dev-only";

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
