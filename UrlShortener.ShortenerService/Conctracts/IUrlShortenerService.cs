namespace UrlShortener.ShortenerService.Contracts;

public interface IUrlShortenerService
{
    /// <summary>
    /// Shortens the given URL.
    /// </summary>
    /// <param name="longUrl"></param>
    /// <returns></returns>
    Task<string> ShortenUrlAsync(string longUrl);
    
    /// <summary>
    /// Gets the original URL from the shortened URL code.
    /// </summary>
    /// <param name="shortUrlCode"></param>
    /// <returns></returns>
    Task<Uri?> GetUrlAsync(string shortUrlCode);

    /// <summary>
    /// Removes the URL from the store.
    /// </summary>
    /// <param name="shortUrlCode"></param>
    /// <returns></returns>
    Task<bool> RemoveUrlAsync(string shortUrlCode);
}
