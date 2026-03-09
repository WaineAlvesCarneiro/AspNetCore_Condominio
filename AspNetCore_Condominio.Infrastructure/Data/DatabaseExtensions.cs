using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Entities.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore_Condominio.Infrastructure.Data;

public static class DatabaseExtensions
{
    public static async Task MigrateAndSeedDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();

        try
        {
            await context.Database.MigrateAsync();

            if (!context.AuthUsers.Any())
            {
                var configuration = services.GetRequiredService<IConfiguration>();

                AuthUser AdminUser = configuration.ToEntityMigrateAndSeedDatabase();

                context.AuthUsers.Add(AdminUser);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<string>>();
            logger.LogError(ex, "Ocorreu um erro ao popular o banco de dados com usuário Admin.");
        }
    }
}