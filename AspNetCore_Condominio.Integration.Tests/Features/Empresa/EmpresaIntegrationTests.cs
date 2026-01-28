extern alias controllerapi;
using controllerapi::AspNetCore_Condominio.API_Controller;

using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Integration.Tests.Commons;
using System.Net;
using System.Net.Http.Json;

namespace AspNetCore_Condominio.Integration.Tests.Features.Empresa
{
    public class EmpresaIntegrationTests : BaseIntegrationTest
    {
        public EmpresaIntegrationTests(CustomWebApplicationFactory<controllerapi::Program> factory) : base(factory) { }
        const string BaseUrl = "/Empresa";

        [Fact]
        public async Task Post_SemToken_DeveRetornarUnauthorized()
        {
            ClearAuthHeader();
            var novoDto = new EmpresaDto {
                RazaoSocial = "Razão Social",
                Fantasia = "Fantasia",
                Cnpj = "01.111.222/0001-02",
                TipoDeCondominio = (TipoCondominio)1,
                Nome = "Responsável",
                Celular = "(11) 99999-9999",
                Telefone = "(11) 3333-3333",
                Email = "email@gmail.com",
                Senha = "SenhaForte123!",
                Host = "smtp.exemplo.com",
                Porta = 587,
                Cep = "01234-567",
                Uf = "SP",
                Cidade = "São Paulo",
                Endereco = "Rua Exemplo, 123",
                Bairro = "Pq Amazônia",
                Complemento = "Complemento",
                DataInclusao = DateTime.UtcNow
            };
            var response = await _client.PostAsJsonAsync(BaseUrl, novoDto);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Post_EmpresaValidoComToken_DeveRetornarCreatedESucesso()
        {
            var seed = await SeedEmpresaAsync(
                "Razão Social",
                "Fantasia",
                "01.111.222/0001-02",
                1,
                "Responsável",
                "(11) 99999-9999",
                "(11) 3333-3333",
                "email@gmail.com",
                "SenhaForte123!",
                "smtp.exemplo.com",
                587,
                "01234-567",
                "SP",
                "São Paulo",
                "Rua Exemplo, 123",
                "Complemento",
                DateTime.Now);
            await AddAdminAuthHeaderAsync();

            var response = await _client.GetAsync($"{BaseUrl}/{seed.Id}");
            response.EnsureSuccessStatusCode();
            var resultDto = await response.Content.ReadFromJsonAsync<Result<EmpresaDto>>();

            Assert.True(resultDto!.Sucesso);
            Assert.Equal("01.111.222/0001-02", resultDto.Dados!.Cnpj);
        }

        [Fact]
        public async Task GetById_EmpresaExistente_DeveRetornarSucesso()
        {
            var seed = await SeedEmpresaAsync(
                "Razão Social",
                "Fantasia",
                "01.111.222/0001-02",
                1,
                "Responsável",
                "(11) 99999-9999",
                "(11) 3333-3333",
                "email@gmail.com",
                "SenhaForte123!",
                "smtp.exemplo.com",
                587,
                "01234-567",
                "SP",
                "São Paulo",
                "Rua Exemplo, 123",
                "Complemento",
                DateTime.Now);
            await AddAdminAuthHeaderAsync();

            var response = await _client.GetAsync($"{BaseUrl}/{seed.Id}");
            response.EnsureSuccessStatusCode();
            var resultDto = await response.Content.ReadFromJsonAsync<Result<EmpresaDto>>();

            Assert.True(resultDto!.Sucesso);
            Assert.Equal(seed.Id, resultDto.Dados!.Id);
        }

        [Fact]
        public async Task Delete_EmpresaExistente_DeveExcluirERetornarSucesso()
        {
            var seed = await SeedEmpresaAsync(
                "Razão Social",
                "Fantasia",
                "01.111.222/0001-02",
                1,
                "Responsável",
                "(11) 99999-9999",
                "(11) 3333-3333",
                "email@gmail.com",
                "SenhaForte123!",
                "smtp.exemplo.com",
                587,
                "01234-567",
                "SP",
                "São Paulo",
                "Rua Exemplo, 123",
                "Complemento",
                DateTime.Now);
            await AddAdminAuthHeaderAsync();
            var response = await _client.DeleteAsync($"{BaseUrl}/{seed.Id}");
            response.EnsureSuccessStatusCode();
            var checkResponse = await _client.GetAsync($"{BaseUrl}/{seed.Id}");

            Assert.Equal(HttpStatusCode.NotFound, checkResponse.StatusCode);
        }
    }
}
