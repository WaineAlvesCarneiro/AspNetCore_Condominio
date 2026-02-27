using AspNetCore_Condominio.API.Endpoints;
using AspNetCore_Condominio.Configurations.Configurations;
using AspNetCore_Condominio.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddAppLogging();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Services.AddAppServices(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ConfigureJsonDefaults());

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.ConfigureJsonDefaults());

builder.Services.AddSwaggerAndSecurity("Condomínio", "Documentação da API do Condomínio desenvolvida em ASP.NET Core.");

var app = builder.Build();

await app.MigrateAndSeedDatabaseAsync();

app.UseAppMiddleware(app.Environment);

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapEnumsEndpoints();

app.Run();

public partial class Program { }