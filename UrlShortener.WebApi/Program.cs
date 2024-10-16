using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using UrlShortener.CodeGenerator;
using UrlShortener.ShortenerService;
using UrlShortener.ShortenerService.Configuration;
using UrlShortener.ShortenerService.Contracts;
using UrlShortener.UrlStore;
using UrlShortener.UrlStore.Contracts;
using Serilog;
using UrlShortener.WebApi.Options;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.Configure<CleanUpOptions>(builder.Configuration.GetSection(CleanUpOptions.ConfigurationSection));
builder.Services.Configure<CodeGeneratorOptions>(builder.Configuration.GetSection(CodeGeneratorOptions.ConfigurationSection));
builder.Services.Configure<UrlShortenerServiceOptions>(builder.Configuration.GetSection(UrlShortenerServiceOptions.ConfigurationSection));

builder.Services.AddDbContext<UrlStoreContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("UrlStoreConnection"));
});

builder.Services.AddScoped<IUrlStore, UrlStoreContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICodeGenerator, AlphanumericalGenerator>();
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("UrlCleanupJob");

    q.AddJob<UrlCleanupJob>(opts => opts.WithIdentity(jobKey));

    var cronSchedule = builder.Configuration
        .GetSection(CleanUpOptions.ConfigurationSection)["CronSchedule"] ?? "0 0 0 * * ?";

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("UrlCleanupJobTrigger")
        .WithCronSchedule(cronSchedule)); 
});

builder.Services.AddQuartzHostedService(q => {
    q.WaitForJobsToComplete = true;
    q.AwaitApplicationStarted = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapControllers();

app.Run();