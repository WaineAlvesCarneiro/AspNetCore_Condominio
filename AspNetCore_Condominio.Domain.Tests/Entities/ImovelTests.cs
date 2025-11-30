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

        //Verifica se a lista/coleção Moradores dentro do objeto imovel agora contém exatamente um item.
        //  Isso garante que a adição ocorreu e que não foram adicionados itens duplicados ou nenhum item.
        Assert.Single(imovel.Moradores);

        // Verifica se o objeto morador criado(João ...) está presente na lista imovel.
        //  Moradores.Isso garante que o item adicionado é o item correto.
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

        /* InvalidOperationException -->
            O objetivo deste teste é garantir a integridade da regra de negócio sobre a capacidade máxima do imóvel:

            O método AdicionarMorador deve lançar uma exceção do tipo InvalidOperationException (ou similar, dependendo da sua implementação) 
                sempre que houver uma tentativa de adicionar um novo morador a um Imovel que já atingiu seu limite máximo de moradores (cinco, neste caso).
        */

        Assert.Throws<InvalidOperationException>(() => imovel.AdicionarMorador(sextoMorador));
    }
}
