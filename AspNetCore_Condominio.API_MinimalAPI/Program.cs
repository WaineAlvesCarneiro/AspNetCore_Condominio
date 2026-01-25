using AspNetCore_Condominio.API_MinimalAPI.Endpoints;
using AspNetCore_Condominio.Configurations.Configurations;
using AspNetCore_Condominio.Configurations.Converters;
using AspNetCore_Condominio.Infrastructure.Data;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddAppLogging();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Services.AddAppServices(builder.Configuration);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonDateOnlyConverter());
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddSwaggerAndSecurity("Condomínio Minimal API", "Documentação da API Minimal de Condomínio desenvolvida em ASP.NET Core 8.");

var app = builder.Build();

await app.MigrateAndSeedDatabaseAsync();

app.UseAppMiddleware(app.Environment);

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapAuthEndpoints();
app.MapImovelEndpoints();
app.MapMoradorEndpoints();

app.Run();

public partial class Program { }