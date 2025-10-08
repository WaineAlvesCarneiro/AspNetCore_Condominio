using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Tests.Entities;

public class ImovelTests
{
    private Imovel CriarImovelBase()
    {
        return new Imovel
        {
            Bloco = "A",
            Apartamento = "101",
            BoxGaragem = "101-A"
        };
    }

    [Fact]
    public void NovoImovel_DeveInicializarListaDeMoradoresVazia()
    {
        var imovel = CriarImovelBase();

        Assert.NotNull(imovel.Moradores);
        Assert.Empty(imovel.Moradores);
    }

    [Fact]
    public void AdicionarMorador_MoradorValido_DeveAdicionarALista()
    {
        var imovel = CriarImovelBase();
        var morador = new Morador { Nome = "João", Email = "joao@a.com", Celular = "99999999999", ImovelId = imovel.Id };
        imovel.AdicionarMorador(morador);

        Assert.Single(imovel.Moradores);
        Assert.Contains(morador, imovel.Moradores);
    }

    [Fact]
    public void AdicionarMorador_MaisDeCincoMoradores_DeveLancarExcecao()
    {
        var imovel = CriarImovelBase();

        for (int i = 1; i <= 5; i++)
        {
            imovel.AdicionarMorador(new Morador { Nome = $"Morador {i}", Email = $"m{i}@a.com", Celular = "99999999999", ImovelId = imovel.Id });
        }

        var sextoMorador = new Morador { Nome = "Sexto", Email = "sexto@a.com", Celular = "99999999999", ImovelId = imovel.Id };

        Assert.Throws<InvalidOperationException>(() => imovel.AdicionarMorador(sextoMorador));
    }
}
