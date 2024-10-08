using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrlShortener.UrlStore.Contracts;
using UrlShortener.UrlStore.Exceptions;
using UrlShortener.UrlStore.Models;

namespace UrlShortener.UrlStore;

/// <summary>
/// Database context for the URL store.
/// </summary>
public class UrlStoreContext : DbContext, IUrlStore
{
    private readonly ILogger<UrlStoreContext> _logger;

    protected DbSet<UrlData> Urls { get; set; }

    public UrlStoreContext(DbContextOptions<UrlStoreContext> options, ILogger<UrlStoreContext> logger)
    : base(options)
    {
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UrlData>(builder =>
        {
            builder.HasKey(url => url.Id);

            builder.HasIndex(url => url.ShortUrlCode).IsUnique();
            builder.HasIndex(url => url.LongUrl).IsUnique();

            builder.Property(url => url.ShortUrlCode).IsRequired();
            builder.Property(url => url.LongUrl)
            .IsRequired()
            .HasConversion(
                uri => uri.ToString(),
                str => new Uri(str)
            );
        });
    }

    /// <summary>
    /// Get the URL data by long URL.
    /// </summary>
    /// <param name="longUrl">The long URL to get the data for.</param>
    /// <returns>The URL data.</returns>
    public async Task<UrlData?> GetByLongUrlAsync(Uri longUrl)
    {
        ArgumentNullException.ThrowIfNull(longUrl);

        return await ExecuteAsync(nameof(GetByLongUrlAsync), async () =>
            await Urls.FirstOrDefaultAsync(url => url.LongUrl == longUrl));
    }

    /// <summary>
    /// Get the URL data by ID.
    /// </summary>
    /// <param name="urlId">The ID of the URL to get the data for.</param>
    /// <returns>The URL data.</returns>
    public async Task<UrlData?> GetAsync(Guid urlId)
    {
        return await ExecuteAsync(nameof(GetAsync), async () =>
            await Urls.FindAsync(urlId));
    }

    /// <summary>
    /// Get the URL data by short URL.
    /// </summary>
    /// <param name="shortUrlCode">The short URL to get the data for.</param>
    /// <returns>The URL data.</returns>
    public async Task<UrlData?> GetByShortUrlAsync(string shortUrlCode)
    {
        ArgumentNullException.ThrowIfNull(shortUrlCode);

        return await ExecuteAsync(nameof(GetByShortUrlAsync), async () =>
            await Urls.FirstOrDefaultAsync(url => url.ShortUrlCode == shortUrlCode));
    }

    /// <summary>
    /// Add a new URL to the store.
    /// </summary>
    /// <param name="urlData">The URL data to add.</param>
    /// <returns>The ID of the added URL.</returns>
    public async Task<Guid> AddAsync(UrlData urlData)
    {
        ArgumentNullException.ThrowIfNull(urlData);
        ArgumentNullException.ThrowIfNull(urlData.LongUrl);
        ArgumentNullException.ThrowIfNull(urlData.ShortUrlCode);

        return await ExecuteAsync(nameof(AddAsync), async () =>
        {
            var existingUrl = await Urls.FirstOrDefaultAsync(url => url.LongUrl == urlData.LongUrl);

            if (existingUrl != null)
            {
                return existingUrl.Id;
            }

            if (await Urls.AnyAsync(url => url.ShortUrlCode == urlData.ShortUrlCode))
                throw new DuplicateShortUrlException($"Short URL '{urlData.ShortUrlCode}' already exists");

            urlData.CreatedAt = DateTime.UtcNow;

            await Urls.AddAsync(urlData);

            // Can break if there are concurrency conflicts... 
            // It could be retried, add optimistic concurrency control, etc.
            await SaveChangesAsync();

            return urlData.Id;
        });
    }

    /// <summary>
    /// Remove a URL from the store.
    /// </summary>
    /// <param name="urlId">The ID of the URL to remove.</param>
    public async Task RemoveAsync(Guid urlId)
    {
        await ExecuteAsync(nameof(RemoveAsync), async () =>
        {
            var url = await Urls.FindAsync(urlId);

            if (url == null)
                return Task.CompletedTask;

            Urls.Remove(url);

            await SaveChangesAsync();

            return Task.CompletedTask;
        });
    }

    private async Task<T> ExecuteAsync<T>(string operationName, Func<Task<T>> operation)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            return await operation();
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation("{OperationName} took {ElapsedMilliseconds}ms", operationName, stopwatch.ElapsedMilliseconds);
        }
    }
}
