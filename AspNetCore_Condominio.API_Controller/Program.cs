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
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonDateOnlyConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddSwaggerAndSecurity("Condomínio API Controller", "Documentação da API Controller de Condomínio desenvolvida em ASP.NET Core 8.");

var app = builder.Build();

await app.MigrateAndSeedDatabaseAsync();

app.UseAppMiddleware(app.Environment);

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

public partial class Program { }