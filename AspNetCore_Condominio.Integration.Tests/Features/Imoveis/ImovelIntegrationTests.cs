extern alias controllerapi;
using controllerapi::AspNetCore_Condominio.API_Controller;

using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Integration.Tests.Commons;
using System.Net;
using System.Net.Http.Json;

namespace AspNetCore_Condominio.Integration.Tests.Features.Imoveis
{
    public class ImovelIntegrationTests : BaseIntegrationTest
    {
        public ImovelIntegrationTests(CustomWebApplicationFactory<controllerapi::Program> factory) : base(factory) { }
        const string BaseUrl = "/Imovel";

        [Fact]
        public async Task Post_SemToken_DeveRetornarUnauthorized()
        {
            ClearAuthHeader();
            var novoDto = new ImovelDto { Bloco = "TEST", Apartamento = "404", BoxGaragem = "G20", EmpresaId = 1 };
            var response = await _client.PostAsJsonAsync(BaseUrl, novoDto);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Post_ValidoComToken_DeveRetornarCreatedESucesso()
        {
            await AddAdminAuthHeaderAsync();
            var novoDto = new ImovelDto { Bloco = "BLOCO", Apartamento = "999", BoxGaragem = "999B", EmpresaId = 1 };
            var response = await _client.PostAsJsonAsync(BaseUrl, novoDto);
            response.EnsureSuccessStatusCode();
            var resultDto = await response.Content.ReadFromJsonAsync<Result<ImovelDto>>();

            Assert.True(resultDto!.Sucesso);
            Assert.Equal("BLOCO", resultDto.Dados!.Bloco);
        }

        [Fact]
        public async Task GetById_Existente_DeveRetornarSucesso()
        {
            var seed = await SeedImovelAsync("BLC_GET", "123", "G123", 1);
            await AddAdminAuthHeaderAsync();

            var response = await _client.GetAsync($"{BaseUrl}/{seed.Id}");
            response.EnsureSuccessStatusCode();
            var resultDto = await response.Content.ReadFromJsonAsync<Result<ImovelDto>>();

            Assert.True(resultDto!.Sucesso);
            Assert.Equal(seed.Id, resultDto.Dados!.Id);
        }

        [Fact]
        public async Task Delete_Existente_DeveExcluirERetornarSucesso()
        {
            var seed = await SeedImovelAsync("BLC_DEL", "999", "G999", 1);
            await AddAdminAuthHeaderAsync();
            var response = await _client.DeleteAsync($"{BaseUrl}/{seed.Id}");
            response.EnsureSuccessStatusCode();
            var checkResponse = await _client.GetAsync($"{BaseUrl}/{seed.Id}");

            Assert.Equal(HttpStatusCode.NotFound, checkResponse.StatusCode);
        }
    }
}
