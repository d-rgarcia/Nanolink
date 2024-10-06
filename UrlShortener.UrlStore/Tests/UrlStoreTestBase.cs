using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using UrlShortener.UrlStore;
using Xunit;

public abstract class UrlStoreTestBase : IAsyncLifetime
{
    private PostgreSqlContainer _postgresContainer;
    protected DbContextOptions<UrlStoreContext> _contextOptions;
    protected readonly ILogger<UrlStoreContext> _logger;

    public UrlStoreTestBase()
    {
        _logger = LoggerFactory.Create(builder => builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug)
            ).CreateLogger<UrlStoreContext>();
    }

    public async Task InitializeAsync()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres")
            .WithDatabase("UrlStoreTest")
            .WithUsername("urlStorer")
            .WithPassword("weakpassword")
            .Build();

        await _postgresContainer.StartAsync();

        _contextOptions = new DbContextOptionsBuilder<UrlStoreContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString())
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .LogTo(message => _logger.LogInformation(message), LogLevel.Information)
            .Options;

        using var context = new UrlStoreContext(_contextOptions, _logger);

        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        using var context = new UrlStoreContext(_contextOptions, _logger);

        await context.Database.EnsureDeletedAsync();

        await _postgresContainer.DisposeAsync();
    }
}