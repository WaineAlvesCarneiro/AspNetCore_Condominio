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
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ConfigureJsonDefaults();
    });

builder.Services.AddSwaggerAndSecurity("Condomínio", "Documentação da API do Condomínio desenvolvida em ASP.NET Core.");

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Condomínio v1"));
}

try {
    await app.MigrateAndSeedDatabaseAsync();
} catch (Exception ex) {
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "Falha ao migrar banco de dados");
}

app.UseAppMiddleware(app.Environment);

app.MapControllers();

app.MapEnumsEndpoints();

app.Run();

public partial class Program { }