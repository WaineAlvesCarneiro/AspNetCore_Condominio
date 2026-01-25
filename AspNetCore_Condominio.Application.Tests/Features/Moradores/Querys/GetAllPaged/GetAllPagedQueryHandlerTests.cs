using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllPaged;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;
using DateTime = System.DateTime;

namespace AspNetCore_Condominio.Application.Tests.Features.Moradores.Querys.GetAllPaged;

public class GetAllPagedQueryHandlerTests
{
    private readonly Mock<IMoradorRepository> _repoMock;
    private readonly GetAllPagedQueryHandlerMorador _handler;

    private readonly List<Morador> _pagina1 = new List<Morador>
    {
        new Morador
        {
            Id = 11, Nome = "Morador 11", Celular = "85991234567", Email = "m11@cond.com", IsProprietario = true,
            DataEntrada = DateOnly.FromDateTime(new DateTime(2023, 11, 1)), DataInclusao = new DateTime(2023, 11, 1),
            ImovelId = 5, EmpresaId = 1, Imovel = new Imovel { Id = 5, Bloco = "C", Apartamento = "301", BoxGaragem = "134" }
        },
        new Morador
        {
            Id = 12, Nome = "Morador 12", Celular = "31991234567", Email = "m12@cond.com", IsProprietario = false,
            DataEntrada = DateOnly.FromDateTime(new DateTime(2023, 11, 2)), DataInclusao = new DateTime(2023, 11, 2), EmpresaId = 1
        }
    };

    private const int Page = 1;
    private const int LinesPerPage = 10;
    private const string? OrderBy = "Id";
    private const string? Direction = "ASC";
    private const string? SearchTerm = null;

    private const int TOTAL_REGISTROS = 25;
    private const int LINES_PER_PAGE = 10;
    private const int PAGE_INDEX = 1;

    public GetAllPagedQueryHandlerTests()
    {
        _repoMock = new Mock<IMoradorRepository>();
        _handler = new GetAllPagedQueryHandlerMorador(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarPagedResultComDadosCorretos()
    {
        // Arrange
        string expectedFirstNome = "Morador 11";

        var query = new GetAllPagedQueryMorador(
            Page: PAGE_INDEX,
            PageSize: LINES_PER_PAGE,
            SortBy: "Id",
            SortDescending: false,
            SearchTerm: null
        );

        _repoMock.Setup(repo => repo.GetAllPagedAsync(Page, LinesPerPage, OrderBy, Direction, SearchTerm)).ReturnsAsync((_pagina1, TOTAL_REGISTROS));

        // Act
        Result<PagedResult<MoradorDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        PagedResult<MoradorDto> pagedResult = resultado.Dados;

        Assert.Equal(TOTAL_REGISTROS, pagedResult.TotalCount);
        Assert.Equal(PAGE_INDEX, pagedResult.PageIndex);
        Assert.Equal(LINES_PER_PAGE, pagedResult.LinesPerPage);
        Assert.Equal(_pagina1.Count, pagedResult.Items.Count());
        Assert.Equal(expectedFirstNome, pagedResult.Items.First().Nome);
        Assert.IsType<PagedResult<MoradorDto>>(pagedResult);

        _repoMock.Verify(repo => repo.GetAllPagedAsync(Page, LinesPerPage, OrderBy, Direction, SearchTerm), Times.Once);

        var primeiroDto = pagedResult.Items.First();

        Assert.Equal("Morador 11", primeiroDto.Nome);
        Assert.Equal(DateOnly.FromDateTime(new DateTime(2023, 11, 1)), primeiroDto.DataEntrada);
        Assert.NotNull(primeiroDto.ImovelDto);
        Assert.Equal("C", primeiroDto.ImovelDto.Bloco);

        _repoMock.Verify(repo => repo.GetAllPagedAsync(Page, LinesPerPage, OrderBy, Direction, SearchTerm), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoRetornaListaVazia_DeveRetornarPagedResultVazio()
    {
        const int totalZero = 0;
        var query = new GetAllPagedQueryMorador();
        _repoMock.Setup(repo => repo.GetAllPagedAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((new List<Morador>(), totalZero));
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(totalZero, resultado.Dados.TotalCount);
        Assert.Empty(resultado.Dados.Items);
    }
}