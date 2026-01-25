using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Infrastructure.Data;
using AspNetCore_Condominio.Integration.Tests.Data;
using AspNetCore_Condominio.Integration.Tests.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace AspNetCore_Condominio.Integration.Tests.Commons
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                services.RemoveAll(typeof(ApplicationDbContext));
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("InMemoryDbForTesting"));
                services.AddScoped<IPasswordHasher<AuthUser>, PasswordHasher<AuthUser>>();

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();

                    try
                    {
                        SeedData.Initialize(db);
                    }
                    catch (Exception ex)
                    {
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();
                        logger.LogError(ex, "Erro ao popular banco de testes");
                    }
                }

                services.RemoveAll<IEmailService>();
                services.AddSingleton<IEmailService, FakeEmailService>();
            });

            builder.UseEnvironment("Testing");
        }
    }
}
