using Microsoft.EntityFrameworkCore;
using UrlShortener.CodeGenerator;
using UrlShortener.ShortenerService;
using UrlShortener.ShortenerService.Configuration;
using UrlShortener.ShortenerService.Contracts;
using UrlShortener.UrlStore;
using UrlShortener.UrlStore.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();