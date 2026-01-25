extern alias controllerapi;
using controllerapi::AspNetCore_Condominio.API_Controller;

using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Integration.Tests.Commons;
using AspNetCore_Condominio.Integration.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace AspNetCore_Condominio.Integration.Tests.Features.Moradores
{
    public class MoradorIntegrationTests : BaseIntegrationTest
    {
        private readonly FakeEmailService _fakeEmailService;

        public MoradorIntegrationTests(CustomWebApplicationFactory<controllerapi::Program> factory) : base(factory) { }
        const string BaseUrl = "/Morador";
        private static long _imovelId = 0;

        private async Task<long> EnsureImovelSeededAsync()
        {
            // Verifique se já existe um imóvel
            var imoveisResponse = await _client.GetAsync("/Imovel");
            if (imoveisResponse.IsSuccessStatusCode)
            {
                var imoveisResult = await imoveisResponse.Content.ReadFromJsonAsync<Result<List<ImovelDto>>>();
                if (imoveisResult?.Dados?.Any() == true)
                {
                    return imoveisResult.Dados.First().Id;
                }
            }

            // Se não existir, crie um imóvel
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
                Nome = $"Teste {Guid.NewGuid():N}",
                Celular = "21999998888",
                Email = $"teste{Guid.NewGuid():N}@email.com",
                IsProprietario = false,
                DataEntrada = DateOnly.FromDateTime(DateTime.UtcNow),
                DataInclusao = DateTime.UtcNow,
                ImovelId = imovelId,
                EmpresaId = 1 // Ou obtenha uma empresa válida
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

        [Fact]
        public async Task GetById_Existente_DeveRetornarSucesso()
        {
            await EnsureImovelSeededAsync();
            await AddAdminAuthHeaderAsync();

            var dto = GetValidMoradorDto(_imovelId);
            var postResponse = await _client.PostAsJsonAsync(BaseUrl, dto);
            postResponse.EnsureSuccessStatusCode();
            var postResult = await postResponse.Content.ReadFromJsonAsync<Result<MoradorDto>>();

            Assert.NotNull(postResult);
            Assert.NotNull(postResult.Dados);

            var createdId = postResult.Dados.Id;
            var getResponse = await _client.GetAsync($"{BaseUrl}/{createdId}");
            getResponse.EnsureSuccessStatusCode();
            var result = await getResponse.Content.ReadFromJsonAsync<Result<MoradorDto>>();

            Assert.Equal(createdId, result!.Dados!.Id);
        }

        [Fact]
        public async Task Delete_existente_DeveExcluirERetornarSucesso()
        {
            await AddAdminAuthHeaderAsync();

            // Use um método de seed para criar o morador
            var moradorId = await SeedMoradorAsync(
                nome: "Alice Teste Delete",
                celular: "21999998888",
                email: $"alice.delete{Guid.NewGuid():N}@teste.com",
                isProprietario: false,
                dataEntrada: DateOnly.FromDateTime(DateTime.UtcNow),
                dataInclusao: DateTime.UtcNow,
                imovelId: await GetOrCreateImovelIdAsync(),
                empresaId: 1);

            // Delete
            var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{moradorId}");
            deleteResponse.EnsureSuccessStatusCode();

            // Verifique
            var checkResponse = await _client.GetAsync($"{BaseUrl}/{moradorId}");
            Assert.Equal(HttpStatusCode.NotFound, checkResponse.StatusCode);
        }

        [Fact]
        public async Task Post_Valido_DeveRetornarCreated_EEnviarEmailDeBoasVindas()
        {
            await EnsureImovelSeededAsync();
            await AddAdminAuthHeaderAsync();

            var emailDestino = $"teste.email@cond.com";
            var dto = GetValidMoradorDto(_imovelId);
            var response = await _client.PostAsJsonAsync(BaseUrl, dto);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result<MoradorDto>>();

            Assert.True(result!.Sucesso);
            Assert.Single(_fakeEmailService.SentEmails);

            var emailEnviado = _fakeEmailService.SentEmails.First();

            Assert.Equal(emailDestino, emailEnviado.To);
            Assert.Contains("Bem-vindo ao Condomínio!", emailEnviado.Subject);
            Assert.Contains($"Olá, **{dto.Nome}**!", emailEnviado.Body);
        }
    }
}