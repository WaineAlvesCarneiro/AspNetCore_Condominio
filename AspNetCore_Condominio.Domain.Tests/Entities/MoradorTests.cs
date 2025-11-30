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
        var mensagemEsperada = "E-mail inválido.";
        var emailEsperadoOriginal = "original@cond.com";

        var morador = CriarMoradorBase();
        var emailInvalido = "emailsemarroba.com";
        var ex = Assert.Throws<ArgumentException>(() => morador.AlterarEmail(emailInvalido));

        Assert.Equal(mensagemEsperada, ex.Message);
        Assert.Equal(emailEsperadoOriginal, morador.Email);
    }

    [Theory] //Indica que este é um método de teste que pode receber dados de diferentes fontes, neste caso, os atributos [InlineData].
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)] //Fornecem os valores de entrada (os "dados de teste") para o parâmetro emailInvalido do método de teste.
                       //O teste será executado três vezes, uma para cada um dos seguintes valores
    public void AlterarEmail_EmailVazioOuNulo_DeveLancarArgumentException(string emailInvalido)
    {
        var morador = CriarMoradorBase();

        //Se o desenvolvedor tentar alterar o e-mail de um Morador para um valor vazio, nulo,
        //  ou apenas com espaços, o sistema deve rejeitar essa operação lançando uma ArgumentException.
        Assert.Throws<ArgumentException>(() => morador.AlterarEmail(emailInvalido));
    }
}