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
}