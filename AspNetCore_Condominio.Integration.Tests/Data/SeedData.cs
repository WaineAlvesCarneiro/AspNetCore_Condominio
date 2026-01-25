using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Infrastructure.Data;

namespace AspNetCore_Condominio.Integration.Tests.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            const string ADMIN_USERNAME = "Admin";
            const string ADMIN_ROLE = "Suporte";
            const string ADMIN_PASSWORD = "12345";

            if (!context.AuthUsers.Any(u => u.UserName == ADMIN_USERNAME))
            {
                var admin = new AuthUser
                {
                    UserName = ADMIN_USERNAME,
                    Role = ADMIN_ROLE,
                    PasswordHash = PasswordHasher.HashPassword(ADMIN_PASSWORD)
                };

                context.AuthUsers.Add(admin);
                context.SaveChanges();

                bool ok = PasswordHasher.VerifyPassword(ADMIN_PASSWORD, admin.PasswordHash);
                Console.WriteLine($"Verificação de senha no seed: {ok}");
            }

            Console.WriteLine($"Usuários no banco: {context.AuthUsers.Count()}");
        }
    }
}
