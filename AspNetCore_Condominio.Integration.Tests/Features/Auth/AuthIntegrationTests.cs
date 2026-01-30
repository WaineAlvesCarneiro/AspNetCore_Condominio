extern alias controllerapi;
using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Integration.Tests.Commons;
using controllerapi::AspNetCore_Condominio.API_Controller;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace AspNetCore_Condominio.Integration.Tests.Features.Auth;

public record AuthLoginRequest(string Username, string Password);

public class AuthIntegrationTests : BaseIntegrationTest
{
    public AuthIntegrationTests(CustomWebApplicationFactory<controllerapi::Program> factory) : base(factory) { }

    [Fact]
    public async Task Post_Login_Valido_Deve_Retornar_Token()
    {
        var loginRequest = new AuthLoginRequest("Admin", "12345");
        var response = await _client.PostAsJsonAsync("/Auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = json.GetProperty("token").GetString();

        Assert.False(string.IsNullOrEmpty(token));
        Assert.True(token.Length > 50, "O token JWT deve ter um comprimento significativo.");
    }

    [Fact]
    public async Task Post_Senha_Incorreta_Deve_Retornar_Falha_De_Autorizacao()
    {
        var loginRequest = new AuthLoginRequest("Admin", "senhaErrada123");
        var response = await _client.PostAsJsonAsync("/Auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Usuario_Inexistente_Deve_Retornar_Falha_De_Autorizacao()
    {
        var loginRequest = new AuthLoginRequest("usuario.inexistente", "12345");
        var response = await _client.PostAsJsonAsync("/Auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private AuthUser CriarBase()
    {
        return new AuthUser
        {
            EmpresaId = 1,
            UserName = "Admin",
            PasswordHash = "12345",
            Role = (TipoRole)1,
            DataInclusao = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };
    }

    [Fact]
    public void Novo_deve_inicializar_lista_de_usuarios_com_um_usuario()
    {
        var dado = CriarBase();

        var dadoParaComparacao = new AuthUser
        {
            EmpresaId = 1,
            UserName = "Admin",
            PasswordHash = "12345",
            Role = (TipoRole)1,
            DataInclusao = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };

        Assert.NotNull(dado);
        Assert.IsType<AuthUser>(dado);
        Assert.Equal(dadoParaComparacao.EmpresaId, dado.EmpresaId);
        Assert.Equal(dadoParaComparacao.UserName, dado.UserName);
        Assert.Equal(dadoParaComparacao.PasswordHash, dado.PasswordHash);
        Assert.Equal(dadoParaComparacao.Role, dado.Role);

        foreach (var prop in dado.GetType().GetProperties())
        {
            var valorEsperado = prop.GetValue(dadoParaComparacao);
            var valorAtual = prop.GetValue(dado);
            Assert.Equal(valorEsperado, valorAtual);
        }

        foreach (var prop in dado.GetType().GetProperties())
        {
            Assert.NotNull(prop.GetValue(dado));
        }
        foreach (var prop in dado.GetType().GetProperties())
        {
            if (prop.PropertyType == typeof(string))
            {
                Assert.False(string.IsNullOrWhiteSpace((string)prop.GetValue(dado)!));
            }
        }
    }

    [Fact]
    public async Task Post_AuthUser_Valido_Com_Token_Deve_Retornar_Created_Sucesso()
    {
        var seed = await SeedAuthUserAsync(
            1,
            "Admin",
            "12345",
            1,
            DateTime.UtcNow);
        await AddAdminAuthHeaderAsync();

        var response = await _client.GetAsync($"{"/Auth/login"}/{seed.Id}");
        response.EnsureSuccessStatusCode();
        var resultDto = await response.Content.ReadFromJsonAsync<Result<AuthUserDto>>();

        Assert.True(resultDto!.Sucesso);
        Assert.Equal("Admin", resultDto.Dados!.UserName);
        Assert.Equal("12345", resultDto.Dados!.PasswordHash);
    }

    [Fact]
    public async Task Get_By_Id_AuthUser_Existente_Deve_Retornar_Sucesso()
    {
        var seed = await SeedAuthUserAsync(
            1,
            "Admin",
            "12345",
            1,
            DateTime.UtcNow);
        await AddAdminAuthHeaderAsync();

        var response = await _client.GetAsync($"{"/Auth/login"}/{seed.Id}");
        response.EnsureSuccessStatusCode();
        var resultDto = await response.Content.ReadFromJsonAsync<Result<AuthUserDto>>();

        Assert.True(resultDto!.Sucesso);
        Assert.Equal(seed.Id, resultDto.Dados!.Id);
    }

    [Fact]
    public async Task Delete_AuthUser_Existente_Deve_Excluir_E_Retornar_Sucesso()
    {
        var seed = await SeedAuthUserAsync(
            1,
            "Admin",
            "12345",
            1,
            DateTime.UtcNow);
        await AddAdminAuthHeaderAsync();
        var response = await _client.DeleteAsync($"{"/Auth/login"}/{seed.Id}");
        response.EnsureSuccessStatusCode();
        var checkResponse = await _client.GetAsync($"{"/Auth/login"}/{seed.Id}");

        Assert.Equal(HttpStatusCode.NotFound, checkResponse.StatusCode);
    }
}
