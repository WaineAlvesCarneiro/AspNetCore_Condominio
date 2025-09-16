using AspNetCore_Condominio.Configurations.Configurations;
using AspNetCore_Condominio.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configura��es consolidadas
builder.Host.AddAppLogging();
builder.Services.AddControllers();
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddSwaggerAndSecurity("Condominio API Controller", "Documenta��o da API Controller de Condom�nio desenvolvida em ASP.NET Core 8.");

var app = builder.Build();

await app.MigrateAndSeedDatabaseAsync();

// Uso de middleware consolidado
app.UseAppMiddleware(app.Environment);

app.MapControllers();

app.Run();