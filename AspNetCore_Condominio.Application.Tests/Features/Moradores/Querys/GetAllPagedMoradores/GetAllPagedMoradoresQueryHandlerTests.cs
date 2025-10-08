using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllPagedMoradores;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;
using DateOnly = System.DateOnly;

namespace AspNetCore_Condominio.Application.Tests.Features.Moradores.Querys.GetAllPagedMoradores;

public class GetAllPagedMoradoresQueryHandlerTests
{
    private readonly Mock<IMoradorRepository> _moradorRepoMock;
    private readonly GetAllPagedMoradoresQueryHandler _handler;

    private const int TOTAL_REGISTROS = 25;
    private const int LINES_PER_PAGE = 10;
    private const int PAGE_INDEX = 1;

    private readonly List<Morador> _moradoresPagina1 = new List<Morador>
    {
        new Morador
        {
            Id = 11, Nome = "Morador 11", Celular = "85991234567", Email = "m11@cond.com", IsProprietario = true,
            DataEntrada = new DateTime(2023, 11, 1), DataInclusao = new DateTime(2023, 11, 1),
            ImovelId = 5, Imovel = new Imovel { Id = 5, Bloco = "C", Apartamento = "301", BoxGaragem = "134" }
        },
        new Morador
        {
            Id = 12, Nome = "Morador 12", Celular = "31991234567", Email = "m12@cond.com", IsProprietario = false,
            DataEntrada = new DateTime(2023, 11, 2), DataInclusao = new DateTime(2023, 11, 2)
        }
    };

    public GetAllPagedMoradoresQueryHandlerTests()
    {
        _moradorRepoMock = new Mock<IMoradorRepository>();
        _handler = new GetAllPagedMoradoresQueryHandler(_moradorRepoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarPagedResultComDadosCorretos()
    {
        var query = new GetAllPagedMoradoresQuery(
            Page: PAGE_INDEX,
            LinesPerPage: LINES_PER_PAGE,
            OrderBy: "Nome",
            Direction: "DESC"
        );

        _moradorRepoMock.Setup(repo => repo.GetAllPagedAsync(
            PAGE_INDEX, LINES_PER_PAGE, "Nome", "DESC")).ReturnsAsync((_moradoresPagina1, TOTAL_REGISTROS));
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        var pagedResult = resultado.Dados;

        Assert.Equal(TOTAL_REGISTROS, pagedResult.TotalCount);
        Assert.Equal(PAGE_INDEX, pagedResult.PageIndex);
        Assert.Equal(LINES_PER_PAGE, pagedResult.LinesPerPage);

        Assert.Equal(_moradoresPagina1.Count, pagedResult.Items.Count());

        var primeiroDto = pagedResult.Items.First();

        Assert.Equal("Morador 11", primeiroDto.Nome);
        Assert.Equal(new DateOnly(2023, 11, 1), primeiroDto.DataEntrada);
        Assert.NotNull(primeiroDto.ImovelDto);
        Assert.Equal("C", primeiroDto.ImovelDto.Bloco);

        _moradorRepoMock.Verify(repo => repo.GetAllPagedAsync(PAGE_INDEX, LINES_PER_PAGE, "Nome", "DESC"), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoRetornaListaVazia_DeveRetornarPagedResultVazio()
    {
        const int totalZero = 0;
        var query = new GetAllPagedMoradoresQuery();
        _moradorRepoMock.Setup(repo => repo.GetAllPagedAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((new List<Morador>(), totalZero));
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(totalZero, resultado.Dados.TotalCount);
        Assert.Empty(resultado.Dados.Items);
    }
}