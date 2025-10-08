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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AspNetCore_Condominio.Integration.Tests.Features.Moradores
{
    public class MoradorIntegrationTests : BaseIntegrationTest
    {
        private readonly FakeEmailService _fakeEmailService;

        public MoradorIntegrationTests(CustomWebApplicationFactory<controllerapi::Program> factory) : base(factory)
        {
            _fakeEmailService = factory.Services.GetRequiredService<IEmailService>() as FakeEmailService
                ?? throw new InvalidOperationException("FakeEmailService não está registrado.");
            _fakeEmailService.SentEmails.Clear();
        }

        private static int _imovelId = 0;

        private async Task<int> EnsureImovelSeededAsync()
        {
            if (_imovelId != 0) return _imovelId;
            Imovel imovelSeed = await SeedImovelAsync("BLC_MORADOR", "505", "G505");
            _imovelId = imovelSeed.Id;
            return _imovelId;
        }

        private MoradorDto GetValidMoradorDto(int imovelId, string? email = null) => new MoradorDto
        {
            Nome = "Alice Teste",
            Celular = "21988887777",
            Email = email ?? $"aliceteste@cond.com",
            IsProprietario = false,
            DataEntrada = DateOnly.FromDateTime(DateTime.UtcNow),
            ImovelId = imovelId
        };

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new DateOnlyJsonConverter() }
        };

        [Fact]
        public async Task Post_MoradorValido_DeveRetornarCreatedESucesso()
        {
            await EnsureImovelSeededAsync();
            await AddAdminAuthHeaderAsync();

            var moradorDto = GetValidMoradorDto(_imovelId);
            var response = await _client.PostAsJsonAsync("/Morador", moradorDto, _jsonOptions);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"POST Morador falhou com status: {response.StatusCode}. Conteúdo: {content}");
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result<MoradorDto>>(_jsonOptions);

            Assert.True(result.Sucesso);
            Assert.NotNull(result.Dados);
            Assert.Equal("Alice Teste", result.Dados.Nome);
        }

        [Fact]
        public async Task GetById_MoradorExistente_DeveRetornarSucesso()
        {
            await EnsureImovelSeededAsync();
            await AddAdminAuthHeaderAsync();

            var moradorDto = GetValidMoradorDto(_imovelId);
            var postResponse = await _client.PostAsJsonAsync("/Morador", moradorDto, _jsonOptions);
            postResponse.EnsureSuccessStatusCode();
            var postResult = await postResponse.Content.ReadFromJsonAsync<Result<MoradorDto>>(_jsonOptions);

            Assert.NotNull(postResult);
            Assert.NotNull(postResult.Dados);

            var createdId = postResult.Dados.Id;
            var getResponse = await _client.GetAsync($"/Morador/{createdId}");
            getResponse.EnsureSuccessStatusCode();
            var result = await getResponse.Content.ReadFromJsonAsync<Result<MoradorDto>>(_jsonOptions);

            Assert.Equal(createdId, result.Dados.Id);
        }

        [Fact]
        public async Task Delete_MoradorExistente_DeveExcluirERetornarSucesso()
        {
            await EnsureImovelSeededAsync();
            await AddAdminAuthHeaderAsync();

            var moradorDto = GetValidMoradorDto(_imovelId);
            var postResponse = await _client.PostAsJsonAsync("/Morador", moradorDto, _jsonOptions);
            postResponse.EnsureSuccessStatusCode();
            var postResult = await postResponse.Content.ReadFromJsonAsync<Result<MoradorDto>>(_jsonOptions);

            Assert.NotNull(postResult);
            Assert.NotNull(postResult.Dados);

            var idToDelete = postResult.Dados.Id;
            var deleteResponse = await _client.DeleteAsync($"/Morador/{idToDelete}");
            deleteResponse.EnsureSuccessStatusCode();
            var checkResponse = await _client.GetAsync($"/Morador/{idToDelete}");

            Assert.Equal(HttpStatusCode.NotFound, checkResponse.StatusCode);
        }

        [Fact]
        public async Task Post_MoradorValido_DeveRetornarCreated_EEnviarEmailDeBoasVindas()
        {
            await EnsureImovelSeededAsync();
            await AddAdminAuthHeaderAsync();

            var emailDestino = $"teste.email@cond.com";
            var moradorDto = GetValidMoradorDto(_imovelId, emailDestino);
            var response = await _client.PostAsJsonAsync("/Morador", moradorDto, _jsonOptions);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result<MoradorDto>>(_jsonOptions);

            Assert.True(result.Sucesso);
            Assert.Single(_fakeEmailService.SentEmails);

            var emailEnviado = _fakeEmailService.SentEmails.First();

            Assert.Equal(emailDestino, emailEnviado.To);
            Assert.Contains("Bem-vindo ao Condomínio!", emailEnviado.Subject);
            Assert.Contains($"Olá, **{moradorDto.Nome}**!", emailEnviado.Body);
        }
    }

    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private readonly string _format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return DateOnly.ParseExact(value!, _format);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }
}