using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Tests.Entities;

public class MoradorCriacaoTests
{
    private const string NOME_VALIDO = "Carla Pereira";
    private const string CELULAR_VALIDO = "99999999999";
    private const string EMAIL_VALIDO = "carla@cond.com";
    private const int IMOVEL_ID_VALIDO = 5;

    [Fact]
    public void Morador_DeveInicializarComDataInclusaoEProprietarioPadrao()
    {
        var morador = new Morador(NOME_VALIDO, CELULAR_VALIDO, EMAIL_VALIDO, IMOVEL_ID_VALIDO)
        {
            Nome = NOME_VALIDO,
            Celular = CELULAR_VALIDO,
            Email = EMAIL_VALIDO,
            ImovelId = IMOVEL_ID_VALIDO,
            DataEntrada = DateTime.Now.AddDays(-1),
            DataInclusao = DateTime.Now.AddDays(-1)
        };

        Assert.NotEqual(default(DateTime), morador.DataInclusao);
        Assert.False(morador.IsProprietario);
        Assert.Null(morador.DataSaida);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CriarMorador_NomeVazio_DeveSerInvalidado(string nomeInvalido)
    {
        var morador = new Morador { Nome = nomeInvalido, Celular = "99999999999", Email = "a@a.com", ImovelId = 1};
    }

}