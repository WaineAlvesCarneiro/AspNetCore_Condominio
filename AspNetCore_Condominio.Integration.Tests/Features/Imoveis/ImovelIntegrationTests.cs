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

        [Fact]
        public async Task Post_SemToken_DeveRetornarUnauthorized()
        {
            ClearAuthHeader();
            var novoImovelDto = new ImovelDto { Bloco = "TEST", Apartamento = "404", BoxGaragem = "G20" };
            var response = await _client.PostAsJsonAsync("/Imovel", novoImovelDto);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Post_ImovelValidoComToken_DeveRetornarCreatedESucesso()
        {
            await AddAdminAuthHeaderAsync();
            var novoImovelDto = new ImovelDto { Bloco = "BLOCO", Apartamento = "999", BoxGaragem = "999B" };
            var response = await _client.PostAsJsonAsync("/Imovel", novoImovelDto);
            response.EnsureSuccessStatusCode();
            var resultDto = await response.Content.ReadFromJsonAsync<Result<ImovelDto>>();

            Assert.True(resultDto.Sucesso);
            Assert.Equal("BLOCO", resultDto.Dados.Bloco);
        }

        [Fact]
        public async Task GetById_ImovelExistente_DeveRetornarSucesso()
        {
            var imovelSeed = await SeedImovelAsync("BLC_GET", "123", "G123");
            await AddAdminAuthHeaderAsync();

            var response = await _client.GetAsync($"/Imovel/{imovelSeed.Id}");
            response.EnsureSuccessStatusCode();
            var resultDto = await response.Content.ReadFromJsonAsync<Result<ImovelDto>>();

            Assert.True(resultDto.Sucesso);
            Assert.Equal(imovelSeed.Id, resultDto.Dados.Id);
        }

        [Fact]
        public async Task Delete_ImovelExistente_DeveExcluirERetornarSucesso()
        {
            var imovelSeed = await SeedImovelAsync("BLC_DEL", "999", "G999");
            await AddAdminAuthHeaderAsync();
            var response = await _client.DeleteAsync($"/Imovel/{imovelSeed.Id}");
            response.EnsureSuccessStatusCode();
            var checkResponse = await _client.GetAsync($"/Imovel/{imovelSeed.Id}");

            Assert.Equal(HttpStatusCode.NotFound, checkResponse.StatusCode);
        }
    }
}
