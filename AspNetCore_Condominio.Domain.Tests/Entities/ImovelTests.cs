using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Tests.Entities;

public class ImovelTests
{
    private Imovel CriarBase()
    {
        return new Imovel
        {
            Bloco = "A",
            Apartamento = "101",
            BoxGaragem = "101-A"
        };
    }

    [Fact]
    public void Novo_DeveInicializarListaDeMoradoresVazia()
    {
        var dado = CriarBase();

        Assert.NotNull(dado.Moradores);
        Assert.Empty(dado.Moradores);
    }

    [Fact]
    public void AdicionarMorador_MoradorValido_DeveAdicionarALista()
    {
        var dado = CriarBase();
        var morador = new Morador { Nome = "João", Email = "joao@a.com", Celular = "99999999999", ImovelId = dado.Id };
        dado.AdicionarMorador(morador);

        Assert.Single(dado.Moradores);
        Assert.Contains(morador, dado.Moradores);
    }

    [Fact]
    public void AdicionarMorador_MaisDeCincoMoradores_DeveLancarExcecao()
    {
        var dado = CriarBase();

        for (int i = 1; i <= 5; i++)
        {
            dado.AdicionarMorador(new Morador { Nome = $"Morador {i}", Email = $"m{i}@a.com", Celular = "99999999999", ImovelId = dado.Id });
        }

        var sextoMorador = new Morador { Nome = "Sexto", Email = "sexto@a.com", Celular = "99999999999", ImovelId = dado.Id };

        Assert.Throws<InvalidOperationException>(() => dado.AdicionarMorador(sextoMorador));
    }
}
