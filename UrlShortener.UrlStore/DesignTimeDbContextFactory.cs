using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using UrlShortener.UrlStore;

/// <summary>
/// Used for EF CLI tooling to create migrations
/// </summary>
public class UrlStoreContextFactory : IDesignTimeDbContextFactory<UrlStoreContext>
{
    public UrlStoreContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UrlStoreContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=my_db;Username=postgres;Password=Admin123");

        var logger = new LoggerFactory().CreateLogger<UrlStoreContext>();

        return new UrlStoreContext(optionsBuilder.Options, logger);
    }
}