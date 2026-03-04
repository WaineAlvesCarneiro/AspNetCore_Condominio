using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
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

                AuthUser AdminUser = MapearEntidade(configuration);

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

    private static AuthUser MapearEntidade(IConfiguration configuration)
    {
        var password = configuration["AdminSettings:Password"] ?? "12345";

        AuthUser AdminUser = new()
        {
            EmpresaId = 0,
            Ativo = TipoUserAtivo.Ativo,
            EmpresaAtiva = TipoEmpresaAtivo.Ativo,
            UserName = configuration["AdminSettings:UserName"] ?? "Admin",
            Email = configuration["AdminSettings:Email"] ?? "enviaemailwebapi@gmail.com",
            PasswordHash = PasswordHasher.HashPassword(password),
            Role = TipoRole.Suporte,
            DataInclusao = DateTime.Now
        };
        return AdminUser;
    }
}