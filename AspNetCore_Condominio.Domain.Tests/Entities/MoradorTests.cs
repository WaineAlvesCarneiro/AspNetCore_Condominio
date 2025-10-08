using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Tests.Entities;

public class MoradorTests
{
    private Morador CriarMoradorBase() => new()
    {
        Nome = "Original",
        Celular = "11999999999",
        Email = "original@cond.com",
        IsProprietario = false,
        DataEntrada = DateTime.Now.AddDays(-10),
        DataInclusao = DateTime.Now.AddDays(-10),
        ImovelId = 1
    };

    [Fact]
    public void AlterarEmail_EmailValido_DeveAtualizarEmailEDataAlteracao()
    {
        var morador = CriarMoradorBase();
        var novoEmail = "novo.email@cond.com";
        var dataOriginalAlteracao = morador.DataAlteracao;
        morador.AlterarEmail(novoEmail);

        Assert.Equal(novoEmail, morador.Email);
        Assert.NotNull(morador.DataAlteracao);
        Assert.NotEqual(dataOriginalAlteracao, morador.DataAlteracao);
    }

    [Fact]
    public void AlterarEmail_EmailInvalidoSemArroba_DeveLancarArgumentException()
    {
        var morador = CriarMoradorBase();
        var emailInvalido = "emailsemarroba.com";
        var ex = Assert.Throws<ArgumentException>(() => morador.AlterarEmail(emailInvalido));

        Assert.Equal("E-mail inválido.", ex.Message);
        Assert.Equal("original@cond.com", morador.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void AlterarEmail_EmailVazioOuNulo_DeveLancarArgumentException(string emailInvalido)
    {
        var morador = CriarMoradorBase();

        Assert.Throws<ArgumentException>(() => morador.AlterarEmail(emailInvalido));
    }
}