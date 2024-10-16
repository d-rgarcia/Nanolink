using Microsoft.Extensions.Options;
using Quartz;
using UrlShortener.UrlStore.Contracts;
using UrlShortener.WebApi.Options;

public class UrlCleanupJob : IJob
{
    private readonly IUrlStore _urlStore;
    private readonly ILogger<UrlCleanupJob> _logger;
    private readonly IOptionsMonitor<CleanUpOptions> _options;

    public UrlCleanupJob(IUrlStore urlStore, ILogger<UrlCleanupJob> logger, IOptionsMonitor<CleanUpOptions> options)
    {
        _urlStore = urlStore;
        _logger = logger;
        _options = options;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        int retentionPeriodDays = _options.CurrentValue.UrlRetentionDays;

        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-retentionPeriodDays);

            int deletedCount = await _urlStore.CleanOldUrls(cutoffDate);

            _logger.LogInformation($"({deletedCount}) URLs cleaned up successfully older than {retentionPeriodDays} days.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cleaning up old URLs.");
        }
    }
}