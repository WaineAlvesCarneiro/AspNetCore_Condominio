extern alias controllerapi;
using controllerapi::AspNetCore_Condominio.API_Controller;

using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
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

        protected async Task<Imovel> SeedImovelAsync(string bloco, string apartamento, string boxGaragem, long empresaId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var dado = new Imovel
                {
                    Bloco = bloco,
                    Apartamento = apartamento,
                    BoxGaragem = boxGaragem,
                    EmpresaId = empresaId
                };

                db.Imovels.Add(dado);
                await db.SaveChangesAsync();
                return dado;
            }
        }

        protected async Task<long> GetOrCreateImovelIdAsync()
        {
            var response = await _client.GetAsync("/Imovel");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Result<List<ImovelDto>>>();
                if (result?.Dados?.Any() == true)
                {
                    return result.Dados.First().Id;
                }
            }

            var imovelDto = new
            {
                Bloco = "A",
                Apartamento = "101",
                BoxGaragem = "1",
                EmpresaId = 1
            };

            var postResponse = await _client.PostAsJsonAsync("/Imovel", imovelDto);
            postResponse.EnsureSuccessStatusCode();

            var postResult = await postResponse.Content.ReadFromJsonAsync<Result<ImovelDto>>();
            Assert.NotNull(postResult?.Dados);

            return postResult.Dados.Id;
        }

        protected async Task<Empresa> SeedEmpresaAsync(string razaoSocial,
                string fantasia,
                string cnpj,
                int tipoDeCondominio,
                string Nome,
                string celular,
                string telefone,
                string email,
                string? senha,
                string host,
                int porta,
                string cep,
                string uf,
                string cidade,
                string endereco,
                string bairro,
                string complemento,
                DateTime dataInclusao)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var dado = new Empresa
                {
                    RazaoSocial = razaoSocial,
                    Fantasia = fantasia,
                    Cnpj = cnpj,
                    TipoDeCondominio = (TipoCondominio)tipoDeCondominio,
                    Nome = Nome,
                    Celular = celular,
                    Telefone = telefone,
                    Email = email,
                    Senha = senha,
                    Host = host,
                    Porta = porta,
                    Cep = cep,
                    Uf = uf,
                    Cidade = cidade,
                    Endereco = endereco,
                    Bairro = bairro,
                    Complemento = complemento,
                    DataInclusao = dataInclusao
                };

                db.Empresas.Add(dado);
                await db.SaveChangesAsync();
                return dado;
            }
        }
    }
}
