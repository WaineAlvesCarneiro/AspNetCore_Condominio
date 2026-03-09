extern alias controllerapi;
using controllerapi::AspNetCore_Condominio.API;

using AspNetCore_Condominio.Application.DTOs;
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
        public async Task Post_Sem_Token_Deve_Retornar_Unauthorized()
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
                DataInclusao = DateTime.Now
            };
            var response = await _client.PostAsJsonAsync(BaseUrl, novoDto);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
