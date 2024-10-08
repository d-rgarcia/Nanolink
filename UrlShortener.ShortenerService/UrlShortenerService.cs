
using UrlShortener.ShortenerService.Contracts;

namespace UrlShortener.ShortenerService;

public class UrlShortenerService : IUrlShortenerService
{
    /// <summary>
    /// Gets the original URL from the shortened URL code.
    /// </summary>
    /// <param name="shortUrlCode"></param>
    /// <returns></returns>
    public Task<string> GetUrlAsync(string shortUrlCode)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Shortens the given URL.
    /// </summary>
    /// <param name="longUrl"></param>
    /// <returns></returns>
    public Task<string> ShortenUrlAsync(string longUrl)
    {
        throw new NotImplementedException();
    }
}
