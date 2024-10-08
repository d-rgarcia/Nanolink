namespace UrlShortener.UrlStore.Models;

/// <summary>
/// DTO about a URL that is stored in the URL store.
/// </summary>
public class UrlData
{   
    /// <summary>
    /// The ID of the URL.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// The long URL that is being shortened.
    /// </summary>
    public required Uri LongUrl { get; set; }

    /// <summary>
    /// The short URL code that is used to access the long URL.
    /// </summary>
    public required string ShortUrlCode { get; set; }

    /// <summary>
    /// The date and time UTC the URL was created.
    /// </summary>
    public DateTime CreatedAt { get; internal set; }
}
