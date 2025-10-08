using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAllPagedImoveis;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Querys.GetAllPagedImoveis;

public class GetAllPagedImoveisQueryHandlerTests
{
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly GetAllPagedImoveisQueryHandler _handler;

    private readonly List<Imovel> _imoveisPagina1 = new List<Imovel>
    {
        new Imovel { Id = 1, Bloco = "A", Apartamento = "101", BoxGaragem = "G1" },
        new Imovel { Id = 2, Bloco = "B", Apartamento = "202", BoxGaragem = "G2" }
    };

    private const int TOTAL_REGISTROS = 15;
    private const int LINES_PER_PAGE = 2;
    private const int PAGE_INDEX = 1;

    public GetAllPagedImoveisQueryHandlerTests()
    {
        _imovelRepoMock = new Mock<IImovelRepository>();
        _handler = new GetAllPagedImoveisQueryHandler(_imovelRepoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarPagedResultComDadosCorretos()
    {
        var query = new GetAllPagedImoveisQuery(
            Page: PAGE_INDEX,
            LinesPerPage: LINES_PER_PAGE,
            OrderBy: "Bloco",
            Direction: "DESC"
        );

        _imovelRepoMock.Setup(repo => repo.GetAllPagedAsync(
            PAGE_INDEX, LINES_PER_PAGE, "Bloco", "DESC")).ReturnsAsync((_imoveisPagina1, TOTAL_REGISTROS));

        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        var pagedResult = resultado.Dados;

        Assert.Equal(TOTAL_REGISTROS, pagedResult.TotalCount);
        Assert.Equal(PAGE_INDEX, pagedResult.PageIndex);
        Assert.Equal(LINES_PER_PAGE, pagedResult.LinesPerPage);
        Assert.Equal(_imoveisPagina1.Count, pagedResult.Items.Count());
        Assert.Equal("A", pagedResult.Items.First().Bloco);
        Assert.IsType<PagedResult<ImovelDto>>(pagedResult);

        _imovelRepoMock.Verify(repo => repo.GetAllPagedAsync(
            PAGE_INDEX, LINES_PER_PAGE, "Bloco", "DESC"), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoRetornaListaVazia_DeveRetornarPagedResultVazio()
    {
        const int totalZero = 0;
        var query = new GetAllPagedImoveisQuery();
        _imovelRepoMock.Setup(repo => repo.GetAllPagedAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((new List<Imovel>(), totalZero));
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(totalZero, resultado.Dados.TotalCount);
        Assert.Empty(resultado.Dados.Items);
    }
}
