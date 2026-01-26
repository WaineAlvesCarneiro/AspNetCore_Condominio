extern alias controllerapi;
using controllerapi::AspNetCore_Condominio.API_Controller;

using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Integration.Tests.Commons;
using System.Net;
using System.Net.Http.Json;

namespace AspNetCore_Condominio.Integration.Tests.Features.Moradores
{
    public class MoradorIntegrationTests : BaseIntegrationTest
    {
        public MoradorIntegrationTests(CustomWebApplicationFactory<controllerapi::Program> factory) : base(factory) { }
        const string BaseUrl = "/Morador";
        private static long _imovelId = 1;

        private async Task<long> EnsureImovelSeededAsync()
        {
            await AddAdminAuthHeaderAsync();

            var imoveisResponse = await _client.GetAsync("/Imovel");
            if (imoveisResponse.IsSuccessStatusCode)
            {
                var imoveisResult = await imoveisResponse.Content.ReadFromJsonAsync<Result<List<ImovelDto>>>();
                if (imoveisResult?.Dados?.Any() == true)
                {
                    return imoveisResult.Dados.First().Id;
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

        private MoradorDto GetValidMoradorDto(long imovelId)
        {
            return new MoradorDto
            {
                Nome = "Alice Teste",
                Celular = "21999998888",
                Email = "testeemail@cond.com",
                IsProprietario = false,
                DataEntrada = DateOnly.FromDateTime(DateTime.UtcNow),
                DataInclusao = DateTime.UtcNow,
                ImovelId = imovelId,
                EmpresaId = 1
            };
        }

        [Fact]
        public async Task Post_Valido_DeveRetornarCreatedESucesso()
        {
            await EnsureImovelSeededAsync();
            await AddAdminAuthHeaderAsync();

            var dto = GetValidMoradorDto(_imovelId);
            var response = await _client.PostAsJsonAsync(BaseUrl, dto);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"POST Morador falhou com status: {response.StatusCode}. Conteúdo: {content}");
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result<MoradorDto>>();

            Assert.True(result!.Sucesso);
            Assert.NotNull(result.Dados);
            Assert.Equal("Alice Teste", result.Dados.Nome);
        }
    }
}