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
    public class AdminSettings
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    private static AdminSettings? adminSettings;

    private static AuthUser AdminUser = new()
    {
        EmpresaId = 0,
        Ativo = TipoUserAtivo.Ativo,
        EmpresaAtiva = TipoEmpresaAtivo.Ativo,
        UserName = adminSettings?.UserName!,
        Email = adminSettings?.Email!,
        PasswordHash = PasswordHasher.HashPassword(adminSettings?.Password!),
        Role = TipoRole.Suporte,
        DataInclusao = DateTime.Now
    };

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
                var config = services.GetRequiredService<IConfiguration>();
                adminSettings = config.GetSection("AdminSettings").Get<AdminSettings>();
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