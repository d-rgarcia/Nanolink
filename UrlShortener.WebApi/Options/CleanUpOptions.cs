namespace UrlShortener.WebApi.Options;

/// <summary>
/// Configuration for clean up job.
/// </summary>
public class CleanUpOptions 
{
    /// <summary>
    /// Identifier name for the section.
    /// </summary>
    public const string ConfigurationSection = "CleanUpOptions";

    /// <summary>
    /// The retention time before clean up old urls.
    /// </summary>
    public int UrlRetentionDays { get; set; } = 70;

    /// <summary>
    /// Schedule for executing the Quartz job.
    /// </summary>
    public string CronSchedule {get; set;} = "0 0 0 * * ?";
}