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

        Assert.NotNull(resultadoPaginado.Items);
        Assert.Empty(resultadoPaginado.Items);
    }
}