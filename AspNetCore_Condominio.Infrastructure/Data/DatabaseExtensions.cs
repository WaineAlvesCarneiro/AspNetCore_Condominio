using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore_Condominio.Infrastructure.Data;

public static class DatabaseExtensions
{
    public static async Task MigrateAndSeedDatabaseAsync(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();

            try
            {
                await context.Database.MigrateAsync();

                if (!context.AuthUsers.Any())
                {
                    string plainPassword = "12345";
                    string hashedPassword = PasswordHasher.HashPassword(plainPassword);

                    var adminUser = new AuthUser
                    {
                        UserName = "Admin",
                        PasswordHash = hashedPassword,
                        Role = (TipoRole)1
                    };

                    context.AuthUsers.Add(adminUser);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Usuário 'Admin' criado com sucesso.");
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<string>>();
                logger.LogError(ex, "Ocorreu um erro ao popular o banco de dados.");
            }
        }
    }
}