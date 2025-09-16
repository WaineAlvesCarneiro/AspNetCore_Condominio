using AspNetCore_Condominio.API_MinimalAPI.Endpoints;
using AspNetCore_Condominio.Configurations.Configurations;
using AspNetCore_Condominio.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configura��es consolidadas
builder.Host.AddAppLogging();
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddSwaggerAndSecurity("Condominio Minimal API", "Documenta��o da API Minimal de Condom�nio desenvolvida em ASP.NET Core 8.");

var app = builder.Build();

await app.MigrateAndSeedDatabaseAsync();

// Uso de middleware consolidado
app.UseAppMiddleware(app.Environment);

app.MapAuthEndpoints();
app.MapImovelEndpoints();
app.MapMoradorEndpoints();

app.Run();
