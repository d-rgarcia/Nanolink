using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using UrlShortener.CodeGenerator;
using UrlShortener.ShortenerService.Configuration;
using UrlShortener.ShortenerService.Contracts;
using UrlShortener.UrlStore.Contracts;
using UrlShortener.UrlStore.Exceptions;
using UrlShortener.UrlStore.Models;

namespace UrlShortener.ShortenerService;

public class UrlShortenerService : IUrlShortenerService
{
    private const int MaxRetryAttempts = 3;
    private const int RetryIntervalInMilliseconds = 500;
    private const string ShortUrlCodeCacheKeyPrefix = "shortUrlCode:";
    private const string LongUrlCacheKeyPrefix = "longUrl:";

    private readonly IMemoryCache _cache;
    private readonly IUrlStore _urlStore;
    private readonly ICodeGenerator _codeGenerator;
    private readonly ILogger<UrlShortenerService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly UrlShortenerServiceOptions _options;

    public UrlShortenerService(IUrlStore urlStore, ICodeGenerator codeGenerator, IMemoryCache cache, IOptions<UrlShortenerServiceOptions> options, ILogger<UrlShortenerService> logger)
    {
        _urlStore = urlStore;
        _codeGenerator = codeGenerator;
        _cache = cache;
        _options = options.Value;
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
    /// Shortens the given URL.
    /// </summary>
    /// <param name="longUrl"></param>
    /// <returns></returns>
    public async Task<string> ShortenUrlAsync(string longUrl)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(longUrl);

        if (!Uri.IsWellFormedUriString(longUrl, UriKind.Absolute))
            throw new ArgumentException("The given URL is not a valid absolute URL.", nameof(longUrl));

        string cacheKey = $"{LongUrlCacheKeyPrefix}{longUrl}";

        if (_cache.TryGetValue(cacheKey, out string? shortUrlCode))
        {
            _logger.LogDebug("Cache hit for long URL: {LongUrl}.", longUrl.ToString());

            if (!string.IsNullOrEmpty(shortUrlCode))
                return shortUrlCode;
        }

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

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(_options.CacheSlidingExpiration)
            .SetAbsoluteExpiration(_options.CacheAbsoluteExpiration);

        _cache.Set(cacheKey, urlData.ShortUrlCode, cacheEntryOptions);

        _logger.LogDebug("Added to long URL cache: {LongUrl}", longUrl.ToString());

        return urlData.ShortUrlCode;
    }

    /// <summary>
    /// Gets the original URL from the shortened URL code.
    /// </summary>
    /// <param name="shortUrlCode"></param>
    /// <returns></returns>
    public async Task<Uri?> GetUrlAsync(string shortUrlCode)
    {
        ArgumentNullException.ThrowIfNull(shortUrlCode);

        string cacheKey = $"{ShortUrlCodeCacheKeyPrefix}{shortUrlCode}";

        if (_cache.TryGetValue(cacheKey, out Uri? longUrl))
        {
            _logger.LogDebug("Cache hit for short URL code: {ShortUrlCode}", shortUrlCode);

            return longUrl;
        }

        _logger.LogDebug("Cache miss for short URL code: {ShortUrlCode}", shortUrlCode);

        var urlData = await _urlStore.GetByShortUrlAsync(shortUrlCode);

        if (urlData != null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_options.CacheSlidingExpiration)
                .SetAbsoluteExpiration(_options.CacheAbsoluteExpiration);

            _cache.Set(cacheKey, urlData.LongUrl, cacheEntryOptions);

            _logger.LogDebug("Added to cache: {ShortUrlCode}", shortUrlCode);
        }
        else
        {
            _logger.LogWarning("No long URL found for the given short URL code: {ShortUrlCode}", shortUrlCode);
        }

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
            string cacheKey = $"{ShortUrlCodeCacheKeyPrefix}{shortUrlCode}";

            _cache.Remove(cacheKey);

            await _urlStore.RemoveAsync(urlData.Id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing URL for the short URL code: {ShortUrlCode}", shortUrlCode);

            return false;
        }
    }
}
