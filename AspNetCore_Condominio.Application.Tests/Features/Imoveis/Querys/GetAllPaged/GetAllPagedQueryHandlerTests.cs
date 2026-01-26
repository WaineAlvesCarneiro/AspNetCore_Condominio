using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAllPaged;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Querys.GetAllPaged;

public class GetAllPagedQueryHandlerTests
{
    private readonly Mock<IImovelRepository> _repoMock;
    private readonly GetAllPagedQueryHandlerImovel _handler;

    private readonly List<Imovel> _pagina1 =
    [
        new Imovel { Id = 1, Bloco = "A", Apartamento = "101", BoxGaragem = "G1", EmpresaId = 1 },
        new Imovel { Id = 2, Bloco = "B", Apartamento = "202", BoxGaragem = "G2", EmpresaId = 1 }
    ];

    private const long UserEmpresaId = 1;
    private const int Page = 1;
    private const int PageSize = 10;
    private const string? SortBy = "Id";
    private const string? SortDescending = "ASC";
    private const string? SearchTerm = null;

    private const int TOTAL_REGISTROS = 2;

    public GetAllPagedQueryHandlerTests()
    {
        _repoMock = new Mock<IImovelRepository>();
        _handler = new GetAllPagedQueryHandlerImovel(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarPagedResultComDadosCorretos()
    {
        // Arrange
        string expectedFirstBloco = "A";
        GetAllPagedQueryImovel query = new(
            UserEmpresaId: UserEmpresaId,
            Page: Page,
            PageSize: PageSize,
            SortBy: SortBy,
            SortDescending: SortDescending!
        );

        _repoMock.Setup(repo => repo.GetAllPagedAsync(
            UserEmpresaId,
            Page,
            PageSize,
            SortBy,
            SortDescending,
            SearchTerm))
            .ReturnsAsync((_pagina1, TOTAL_REGISTROS));

        // Act
        Result<PagedResult<ImovelDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        PagedResult<ImovelDto> pagedResult = resultado.Dados;

        Assert.Equal(TOTAL_REGISTROS, pagedResult.TotalCount);
        Assert.Equal(Page, pagedResult.PageIndex);
        Assert.Equal(PageSize, pagedResult.PageSize);
        Assert.Equal(_pagina1.Count, pagedResult.Items.Count());
        Assert.Equal(expectedFirstBloco, pagedResult.Items.First().Bloco);
        Assert.IsType<PagedResult<ImovelDto>>(pagedResult);

        _repoMock.Verify(repo => repo.GetAllPagedAsync(
            UserEmpresaId,
            Page,
            PageSize,
            SortBy,
            SortDescending,
            SearchTerm), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoRetornaListaVazia_DeveRetornarPagedResultVazio()
    {
        const int totalZero = 0;
        GetAllPagedQueryImovel query = new(UserEmpresaId);
        _repoMock.Setup(repo => repo.GetAllPagedAsync(
            It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((new List<Imovel>(), totalZero));
        Result<PagedResult<ImovelDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(totalZero, resultado.Dados.TotalCount);
        Assert.Empty(resultado.Dados.Items);
    }
}
