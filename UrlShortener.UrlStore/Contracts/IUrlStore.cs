using UrlShortener.UrlStore.Models;

namespace UrlShortener.UrlStore.Contracts;

/// <summary>
/// Interface for a URL store.
/// </summary>
public interface IUrlStore
{
    /// <summary>
    /// Gets the URL data for the given long URL.
    /// </summary>
    /// <param name="longUrl">The long URL.</param>
    /// <returns>The URL data.</returns>
    Task<UrlData?> GetByLongUrlAsync(Uri longUrl);

    /// <summary>
    /// Gets the URL data for the given URL ID.
    /// </summary>
    /// <param name="urlId">The ID of the URL.</param>
    /// <returns>The URL data.</returns>
    Task<UrlData?> GetAsync(Guid urlId);

    /// <summary>
    /// Gets the URL data for the given short URL.
    /// </summary>
    /// <param name="shortUrl">The short URL.</param>
    /// <returns>The URL data.</returns>
    Task<UrlData?> GetByShortUrlAsync(Uri shortUrl);

    /// <summary>
    /// Tries to add a new URL to the store.
    /// </summary>
    /// <param name="urlData">The URL data to add.</param>
    /// <returns>The ID of the new URL.</returns>
    Task<Guid> AddAsync(UrlData urlData);

    /// <summary>
    /// Removes a URL from the store.
    /// </summary>
    /// <param name="urlId">The ID of the URL to remove.</param>
    Task RemoveAsync(Guid urlId);
}