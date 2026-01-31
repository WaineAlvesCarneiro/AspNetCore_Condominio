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
            const long empresaId = 0;
            const string username = "Admin";
            const string email = "enviaemailwebapi@gmail.com";
            const string plainPassword = "12345";
            const int roleValue = 1;

            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();

            try
            {
                await context.Database.MigrateAsync();

                if (!context.AuthUsers.Any())
                {
                    string hashedPassword = PasswordHasher.HashPassword(plainPassword.ToString());

                    var adminUser = new AuthUser
                    {
                        EmpresaId = empresaId,
                        UserName = username,
                        Email = email,
                        PasswordHash = hashedPassword,
                        Role = (TipoRole)roleValue,
                        DataInclusao = DateTime.Now
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