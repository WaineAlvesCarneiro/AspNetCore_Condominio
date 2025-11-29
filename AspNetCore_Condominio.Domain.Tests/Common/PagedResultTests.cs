using AspNetCore_Condominio.Domain.Common;

namespace AspNetCore_Condominio.Domain.Tests.Common;

public class PagedResultTests
{
    [Fact]
    public void PagedResult_DeveInicializarCorretamente_ComDadosEMetadados()
    {
        var itensDeTeste = new List<string> { "Item A", "Item B", "Item C" };
        var totalDeItens = 45;
        var indiceDaPagina = 2;
        var linhasPorPagina = 10;
        var totalresultadoPaginado = 3;
        var itemB = "Item B";

        var resultadoPaginado = new PagedResult<string>
        {
            Items = itensDeTeste,
            TotalCount = totalDeItens,
            PageIndex = indiceDaPagina,
            LinesPerPage = linhasPorPagina
        };

        Assert.Equal(totalDeItens, resultadoPaginado.TotalCount);
        Assert.Equal(indiceDaPagina, resultadoPaginado.PageIndex);
        Assert.Equal(linhasPorPagina, resultadoPaginado.LinesPerPage);
        Assert.NotNull(resultadoPaginado.Items);
        Assert.Equal(totalresultadoPaginado, resultadoPaginado.Items.Count());
        Assert.Contains(itemB, resultadoPaginado.Items);
    }

    [Fact]
    public void PagedResult_DeveInicializarItemsComoListaVazia()
    {
        var resultadoPaginado = new PagedResult<int>();

        // Afirma que o objeto 'resultadoPaginado.Items' foi criado com sucesso e não é nulo
        // garantindo que o seu código não tente acessar membros de um objeto que não existe, o que resultaria em um erro de
        //  NullReferenceException
        Assert.NotNull(resultadoPaginado.Items);

        //verificar se uma coleção (como uma lista, array ou string) está vazia.
        Assert.Empty(resultadoPaginado.Items);
    }
}