using Microsoft.AspNetCore.Mvc;
using UrlShortener.ShortenerService.Contracts;

namespace UrlShortener.WebApi.Controllers;

[ApiController]
[Route("/")]
public class UrlShortenerController : ControllerBase
{
    private readonly IUrlShortenerService _urlShortenerService;
    private readonly ILogger<UrlShortenerController> _logger;

    public UrlShortenerController(IUrlShortenerService urlShortenerService, ILogger<UrlShortenerController> logger)
    {
        _urlShortenerService = urlShortenerService;
        _logger = logger;
    }

    [HttpPost("ShortenUrlAsync")]
    public async Task<IActionResult> ShortenUrlAsync([FromBody] string longUrl)
    {
        _logger.LogInformation("Shortening URL: {LongUrl}", longUrl);

        if (string.IsNullOrWhiteSpace(longUrl))
        {
            _logger.LogWarning("Invalid URL: {LongUrl}", longUrl);

            return BadRequest("The URL cannot be empty.");
        }

        try
        {
            var shortCode = await _urlShortenerService.ShortenUrlAsync(longUrl);

            if (shortCode == null)
            {
                _logger.LogError("Error shortening URL: {LongUrl}", longUrl);

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while shortening the URL.");
            }

            var shortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}";

            return Ok(shortUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error shortening URL: {LongUrl}", longUrl);

            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("{shortUrlCode}")]
    public async Task<IActionResult> RedirectTo(string shortUrlCode)
    {
        _logger.LogInformation("Redirecting to long URL for short URL code: {ShortUrlCode}", shortUrlCode);

        if (string.IsNullOrEmpty(shortUrlCode))
        {
            _logger.LogWarning("Invalid short URL code: {ShortUrlCode}", shortUrlCode);

            return BadRequest("The short URL code cannot be empty.");
        }

        try
        {
            var longUrl = await _urlShortenerService.GetUrlAsync(shortUrlCode);

            if (longUrl == null)
                return NotFound("The requested URL was not found.");

            return Redirect(longUrl.AbsoluteUri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redirecting to long URL for short URL code: {ShortUrlCode}", shortUrlCode);

            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }
}
