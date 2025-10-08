using AspNetCore_Condominio.Domain.Entities.EmailRemetente;

namespace AspNetCore_Condominio.Domain.Tests.Entities.EmailRemetenteTests;

public class EmailRemetenteTests
{
    private const string USERNAME = "teste";
    private const string SENHA = "senha";
    private const string HOST = "smtp.host.com";
    private const int PORT = 587;

    [Fact]
    public void EmailRemetente_ComPortaValida_DeveSerCriadoCorretamente()
    {
        var remetente = new EmailRemetente(USERNAME, SENHA, HOST, PORT)
        {
            Username = USERNAME,
            Senha = SENHA,
            Host = HOST,
            Port = PORT
        };

        Assert.Equal(587, remetente.Port);
        Assert.Equal(USERNAME, remetente.Username);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void EmailRemetente_ComPortaInvalida_DeveLancarArgumentException(int portaInvalida)
    {
        var ex = Assert.Throws<ArgumentException>(
            () => new EmailRemetente(USERNAME, SENHA, HOST, portaInvalida)
            {
                Username = USERNAME,
                Senha = SENHA,
                Host = HOST,
                Port = PORT
            }
        );

        Assert.Contains("A porta deve ser um número positivo.", ex.Message);
    }
}