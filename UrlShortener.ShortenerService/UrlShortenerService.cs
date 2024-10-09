using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using UrlShortener.CodeGenerator;
using UrlShortener.ShortenerService.Contracts;
using UrlShortener.UrlStore.Contracts;
using UrlShortener.UrlStore.Exceptions;
using UrlShortener.UrlStore.Models;

namespace UrlShortener.ShortenerService;

public class UrlShortenerService : IUrlShortenerService
{
    private const int MaxRetryAttempts = 3;
    private const int RetryIntervalInMilliseconds = 500;

    private readonly IUrlStore _urlStore;
    private readonly ICodeGenerator _codeGenerator;
    private readonly ILogger<UrlShortenerService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    public UrlShortenerService(IUrlStore urlStore, ICodeGenerator codeGenerator, ILogger<UrlShortenerService> logger)
    {
        _urlStore = urlStore;
        _codeGenerator = codeGenerator;
        _logger = logger;
        _retryPolicy = Policy
            .Handle<DuplicateShortUrlException>()
            .WaitAndRetryAsync(MaxRetryAttempts,
                retryAttempt => TimeSpan.FromMilliseconds(RetryIntervalInMilliseconds * Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "Retry {RetryCount} for generating short URL code at {TimeSpan}ms", retryCount, timeSpan.TotalMilliseconds);
                });
    }

    /// <summary>
    /// Gets the original URL from the shortened URL code.
    /// </summary>
    /// <param name="shortUrlCode"></param>
    /// <returns></returns>
    public async Task<Uri?> GetUrlAsync(string shortUrlCode)
    {
        ArgumentNullException.ThrowIfNull(shortUrlCode);

        var urlData = await _urlStore.GetByShortUrlAsync(shortUrlCode);

        if (urlData == null)
            _logger.LogWarning("No long URL found for the given short URL code: {ShortUrlCode}", shortUrlCode);

        return urlData?.LongUrl;
    }

    /// <summary>
    /// Removes the URL from the store.
    /// </summary>
    /// <param name="shortUrlCode"></param>
    /// <returns></returns>
    public async Task<bool> RemoveUrlAsync(string shortUrlCode)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(shortUrlCode);

        var urlData = await _urlStore.GetByShortUrlAsync(shortUrlCode);

        if (urlData == null)
        {
            _logger.LogWarning("Can not find the URL to remove for the short URL code: {ShortUrlCode}", shortUrlCode);

            return false;
        }

        try
        {
            await _urlStore.RemoveAsync(urlData.Id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing URL for the short URL code: {ShortUrlCode}", shortUrlCode);

            return false;
        }
    }

    /// <summary>
    /// Shortens the given URL.
    /// </summary>
    /// <param name="longUrl"></param>
    /// <returns></returns>
    public async Task<string> ShortenUrlAsync(string longUrl)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(longUrl);

        if (!Uri.IsWellFormedUriString(longUrl, UriKind.Absolute))
            throw new ArgumentException("The given URL is not a valid absolute URL.", nameof(longUrl));

        var urlData = await _urlStore.GetByLongUrlAsync(new Uri(longUrl));

        if (urlData == null)
        {
            urlData = await _retryPolicy.ExecuteAsync(async () =>
            {
                var shortUrlCode = _codeGenerator.GenerateCode();

                urlData = new UrlData
                {
                    LongUrl = new Uri(longUrl),
                    ShortUrlCode = shortUrlCode
                };

                await _urlStore.AddAsync(urlData);

                return urlData;
            });
        }

        return urlData.ShortUrlCode;
    }
}
