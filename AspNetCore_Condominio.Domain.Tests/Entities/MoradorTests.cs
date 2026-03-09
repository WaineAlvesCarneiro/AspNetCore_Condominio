using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Tests.Entities;

public class MoradorTests
{
    private const long UserEmpresaId = 1;

    private Morador CriarBase() => new()
    {
        Nome = "Original",
        Celular = "11999999999",
        Email = "original@cond.com",
        IsProprietario = false,
        DataEntrada = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)),
        DataInclusao = DateTime.Now.AddDays(-10),
        ImovelId = 1,
        EmpresaId = UserEmpresaId
    };

    [Fact]
    public void AlterarEmail_EmailValido_DeveAtualizarEmailEDataAlteracao()
    {
        var dado = CriarBase();
        var novoEmail = "novo.email@cond.com";
        var dataOriginalAlteracao = dado.DataAlteracao;
        dado.AlterarEmail(novoEmail);

        Assert.Equal(novoEmail, dado.Email);
        Assert.NotNull(dado.DataAlteracao);
        Assert.NotEqual(dataOriginalAlteracao, dado.DataAlteracao);
    }

    [Fact]
    public void AlterarEmail_EmailInvalidoSemArroba_DeveLancarArgumentException()
    {
        var mensagemEsperada = "E-mail inválido.";
        var emailEsperadoOriginal = "original@cond.com";

        var dado = CriarBase();
        var emailInvalido = "emailsemarroba.com";
        var ex = Assert.Throws<ArgumentException>(() => dado.AlterarEmail(emailInvalido));

        Assert.Equal(mensagemEsperada, ex.Message);
        Assert.Equal(emailEsperadoOriginal, dado.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void AlterarEmail_EmailVazioOuNulo_DeveLancarArgumentException(string emailInvalido)
    {
        var dado = CriarBase();
        Assert.Throws<ArgumentException>(() => dado.AlterarEmail(emailInvalido));
    }
}