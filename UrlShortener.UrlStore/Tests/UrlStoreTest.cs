using UrlShortener.UrlStore;
using UrlShortener.UrlStore.Exceptions;
using UrlShortener.UrlStore.Models;
using Xunit;

public class UrlStoreTest : UrlStoreTestBase
{
    [Fact]
    public async Task AddAsync_DuplicateLongUrl_ReturnsSameId()
    {
        var firstUrl = new UrlData
        {
            LongUrl = new Uri("https://www.google.com"),
            ShortUrlCode = "abc123"
        };

        var secondUrl = new UrlData
        {
            LongUrl = new Uri("https://www.google.com"),
            ShortUrlCode = "def456"
        };

        using var urlStore = new UrlStoreContext(_contextOptions, _logger);

        var createdFirstId = await urlStore.AddAsync(firstUrl);
        var createdSecondId = await urlStore.AddAsync(secondUrl);

        var result = await urlStore.GetByLongUrlAsync(secondUrl.LongUrl);

        Assert.NotNull(result);
        Assert.Equal(result.Id, firstUrl.Id);
        Assert.Equal(result.Id, createdFirstId);
        Assert.Equal(result.Id, createdSecondId);
        Assert.Equal(result.ShortUrlCode, firstUrl.ShortUrlCode);
    }

    [Fact]
    public async Task GetAndRemoveAsync_ExistingUrl_SucceedsAndThenReturnsNull()
    {
        var url = new UrlData
        {
            LongUrl = new Uri("https://www.google.com"),
            ShortUrlCode = "abc123"
        };

        using var urlStore = new UrlStoreContext(_contextOptions, _logger);

        var createdId = await urlStore.AddAsync(url);

        var result = await urlStore.GetAsync(url.Id);

        Assert.NotNull(result);
        Assert.Equal(result.ShortUrlCode, url.ShortUrlCode);
        Assert.Equal(result.LongUrl, url.LongUrl);
        Assert.Equal(result.Id, createdId);

        await urlStore.RemoveAsync(url.Id);

        result = await urlStore.GetAsync(url.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByShortUrlAsync_ExistingUrl_ReturnsCorrectUrl()
    {
        var url = new UrlData
        {
            LongUrl = new Uri("https://www.google.com"),
            ShortUrlCode = "abc123"
        };

        using var urlStore = new UrlStoreContext(_contextOptions, _logger);

        var createdId = await urlStore.AddAsync(url);

        var result = await urlStore.GetByShortUrlAsync(url.ShortUrlCode);

        Assert.NotNull(result);
        Assert.Equal(result.Id, createdId);
        Assert.Equal(result.ShortUrlCode, url.ShortUrlCode);
        Assert.Equal(result.LongUrl, url.LongUrl);
    }

    [Fact]
    public async Task AddAsync_DuplicateShortUrl_ThrowsDuplicateShortUrlException()
    {
        var url = new UrlData
        {
            LongUrl = new Uri("https://www.google.com"),
            ShortUrlCode = "abc123"
        };

        using var urlStore = new UrlStoreContext(_contextOptions, _logger);

        await urlStore.AddAsync(url);

        var secondUrl = new UrlData
        {
            LongUrl = new Uri("https://www.bing.com"),
            ShortUrlCode = url.ShortUrlCode
        };

        await Assert.ThrowsAsync<DuplicateShortUrlException>(() => urlStore.AddAsync(secondUrl));
    }

    [Fact]
    public async Task CleanOldUrls_OlderThanGiven_RemovesMatchedUrls()
    {
        var young = new UrlData
        {
            LongUrl = new Uri("https://www.google.com"),
            ShortUrlCode = "young",
            LastAccessedAt = DateTime.UtcNow
        };
        var oldUrl1 = new UrlData
        {
            LongUrl = new Uri("https://www.yahoo.com"),
            ShortUrlCode = "oldman",
            LastAccessedAt = DateTime.UtcNow.AddDays(-90)
        };
        var oldUrl2 = new UrlData
        {
            LongUrl = new Uri("https://www.bing.com"),
            ShortUrlCode = "oldman2",
            LastAccessedAt = DateTime.UtcNow.AddDays(-30)
        };
        
        using var urlStore = new UrlStoreContext(_contextOptions, _logger);

        urlStore.Add(young);
        urlStore.Add(oldUrl1);
        urlStore.Add(oldUrl2);

        urlStore.SaveChanges();

        int count = await urlStore.CleanOldUrls(DateTime.UtcNow.AddDays(-30));

        Assert.NotNull(await urlStore.GetByShortUrlAsync(young.ShortUrlCode));
        Assert.Null(await urlStore.GetByShortUrlAsync(oldUrl1.ShortUrlCode));
        Assert.Null(await urlStore.GetByShortUrlAsync(oldUrl1.ShortUrlCode));
        Assert.Equal(2, count);
    }
}