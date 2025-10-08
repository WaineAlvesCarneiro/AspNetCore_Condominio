extern alias controllerapi;
using controllerapi::AspNetCore_Condominio.API_Controller;

using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace AspNetCore_Condominio.Integration.Tests.Commons
{
    public class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory<controllerapi::Program>>
    {
        protected readonly HttpClient _client;
        protected readonly CustomWebApplicationFactory<controllerapi::Program> _factory;

        public BaseIntegrationTest(CustomWebApplicationFactory<controllerapi::Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        protected async Task AddAdminAuthHeaderAsync()
        {
            var loginRequest = new AuthLoginRequest("admin", "12345");
            var response = await _client.PostAsJsonAsync("/Auth/login", loginRequest);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var token = json.GetProperty("token").GetString();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected void ClearAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization = null;
        }

        protected async Task<Imovel> SeedImovelAsync(string bloco, string apartamento, string boxGaragem)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var imovel = new Imovel
                {
                    Bloco = bloco,
                    Apartamento = apartamento,
                    BoxGaragem = boxGaragem
                };

                db.Imovels.Add(imovel);
                await db.SaveChangesAsync();
                return imovel;
            }
        }
    }
}
