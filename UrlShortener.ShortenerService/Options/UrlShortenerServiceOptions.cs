using System;

namespace UrlShortener.ShortenerService.Configuration;

/// <summary>
/// Options for the URL shortener service.
/// </summary>
public class UrlShortenerServiceOptions
{
    /// <summary>
    /// The configuration section for the URL shortener service options.
    /// </summary>
    public const string ConfigurationSection = "UrlShortenerService";

    /// <summary>
    /// The sliding expiration for the cache.
    /// </summary>
    public TimeSpan CacheSlidingExpiration { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// The absolute expiration for the cache.
    /// </summary>
    public TimeSpan CacheAbsoluteExpiration { get; set; } = TimeSpan.FromHours(1);
}