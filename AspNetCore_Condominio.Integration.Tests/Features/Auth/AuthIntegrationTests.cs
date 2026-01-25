extern alias controllerapi;
using controllerapi::AspNetCore_Condominio.API_Controller;

using AspNetCore_Condominio.Integration.Tests.Commons;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace AspNetCore_Condominio.Integration.Tests.Features.Auth
{
    public record AuthLoginRequest(string Username, string Password);

    public class AuthIntegrationTests : BaseIntegrationTest
    {
        public AuthIntegrationTests(CustomWebApplicationFactory<controllerapi::Program> factory) : base(factory) { }

        [Fact]
        public async Task Post_LoginValido_DeveRetornarToken()
        {
            var loginRequest = new AuthLoginRequest("admin", "12345");
            var response = await _client.PostAsJsonAsync("/Auth/login", loginRequest);
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var token = json.GetProperty("token").GetString();

            Assert.False(string.IsNullOrEmpty(token));
            Assert.True(token.Length > 50, "O token JWT deve ter um comprimento significativo.");
        }

        [Fact]
        public async Task Post_SenhaIncorreta_DeveRetornarFalhaDeAutorizacao()
        {
            var loginRequest = new AuthLoginRequest("admin", "senhaErrada123");
            var response = await _client.PostAsJsonAsync("/Auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Post_UsuarioInexistente_DeveRetornarFalhaDeAutorizacao()
        {
            var loginRequest = new AuthLoginRequest("usuario.inexistente", "12345");
            var response = await _client.PostAsJsonAsync("/Auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
