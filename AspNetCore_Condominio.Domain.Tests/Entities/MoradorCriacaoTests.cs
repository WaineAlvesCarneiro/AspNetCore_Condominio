using AspNetCore_Condominio.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore_Condominio.Domain.Tests.Entities;

public class MoradorCriacaoTests
{
    private const string NOME_VALIDO = "Carla Pereira";
    private const string CELULAR_VALIDO = "99999999999";
    private const string EMAIL_VALIDO = "carla@cond.com";
    private const int IMOVEL_ID_VALIDO = 5;
    private readonly DateTime DATAENTRADA = DateTime.Now.AddDays(-1);
    private readonly DateTime DATAINCLUSAO = DateTime.Now.AddDays(-1);

    [Fact]
    public void Morador_DeveInicializarComDataInclusaoEProprietarioPadrao()
    {
        var morador = new Morador(NOME_VALIDO, CELULAR_VALIDO, EMAIL_VALIDO, IMOVEL_ID_VALIDO)
        {
            Nome = NOME_VALIDO,
            Celular = CELULAR_VALIDO,
            Email = EMAIL_VALIDO,
            ImovelId = IMOVEL_ID_VALIDO,
            DataEntrada = DATAENTRADA,
            DataInclusao = DATAINCLUSAO
        };

        Assert.Equal(NOME_VALIDO, morador.Nome);
        Assert.Equal(CELULAR_VALIDO, morador.Celular);
        Assert.Equal(EMAIL_VALIDO, morador.Email);
        Assert.Equal(DATAENTRADA, morador.DataEntrada);
        Assert.Equal(IMOVEL_ID_VALIDO, morador.ImovelId);
        Assert.Equal(DATAINCLUSAO, morador.DataInclusao);
        Assert.Null(morador.DataSaida);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CriarMorador_NomeVazio_DeveSerInvalidado(string nomeInvalido)
    {
        var morador = new Morador { Nome = nomeInvalido, Celular = "99999999999", Email = "a@a.com", ImovelId = 1};

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(morador);
        bool isValid = Validator.TryValidateObject(morador, validationContext, validationResults, true);
        Assert.True(isValid);
        Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Contains("Nome"));
        Assert.DoesNotContain(validationResults, vr => vr.ErrorMessage!.Contains("required"));
    }
}