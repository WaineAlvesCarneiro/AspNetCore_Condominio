using AspNetCore_Condominio.Configurations.Configurations;
using AspNetCore_Condominio.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurações consolidadas
builder.Host.AddAppLogging();
builder.Services.AddControllers();
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddSwaggerAndSecurity("Condominio API Controller", "Documentação da API Controller de Condomínio desenvolvida em ASP.NET Core 8.");

var app = builder.Build();

await app.MigrateAndSeedDatabaseAsync();

// Uso de middleware consolidado
app.UseAppMiddleware(app.Environment);

app.MapControllers();

app.Run();