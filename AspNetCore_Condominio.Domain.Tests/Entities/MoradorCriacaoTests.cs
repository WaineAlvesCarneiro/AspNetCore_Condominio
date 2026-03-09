using AspNetCore_Condominio.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore_Condominio.Domain.Tests.Entities;

public class MoradorCriacaoTests
{
    private const string NOME_VALIDO = "Carla Pereira";
    private const string CELULAR_VALIDO = "99999999999";
    private const string EMAIL_VALIDO = "carla@cond.com";
    private const int IMOVEL_ID_VALIDO = 5;
    private const long UserEmpresaId = 1;
    private readonly DateOnly DATAENTRADA = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
    private readonly DateTime DATAINCLUSAO = DateTime.Now.AddDays(-1);
    private readonly bool ISPROPRIETARIO = false;

    [Fact]
    public void DeveInicializarComDataInclusaoEProprietarioPadrao()
    {
        var dado = new Morador(NOME_VALIDO, CELULAR_VALIDO, EMAIL_VALIDO, IMOVEL_ID_VALIDO, UserEmpresaId, DATAENTRADA, ISPROPRIETARIO)
        {
            Nome = NOME_VALIDO,
            Celular = CELULAR_VALIDO,
            Email = EMAIL_VALIDO,
            ImovelId = IMOVEL_ID_VALIDO,
            EmpresaId = UserEmpresaId,
            DataEntrada = DATAENTRADA,
            DataInclusao = DATAINCLUSAO,
            IsProprietario = ISPROPRIETARIO
        };

        Assert.Equal(NOME_VALIDO, dado.Nome);
        Assert.Equal(CELULAR_VALIDO, dado.Celular);
        Assert.Equal(EMAIL_VALIDO, dado.Email);
        Assert.Equal(DATAENTRADA, dado.DataEntrada);
        Assert.Equal(IMOVEL_ID_VALIDO, dado.ImovelId);
        Assert.Equal(UserEmpresaId, dado.EmpresaId);
        Assert.Equal(DATAINCLUSAO, dado.DataInclusao);
        Assert.Equal(ISPROPRIETARIO, dado.IsProprietario);
        Assert.Null(dado.DataSaida);
        Assert.Null(dado.DataAlteracao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Criar_NomeVazio_DeveSerInvalidado(string nomeInvalido)
    {
        var dado = new Morador { Nome = nomeInvalido, Celular = "99999999999", Email = "a@a.com", ImovelId = 1, EmpresaId = UserEmpresaId };

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dado);
        bool isValid = Validator.TryValidateObject(dado, validationContext, validationResults, true);
        Assert.True(isValid);
        Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Contains("Nome"));
        Assert.DoesNotContain(validationResults, vr => vr.ErrorMessage!.Contains("required"));
    }
}