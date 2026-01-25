using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAll;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Querys.GetAll;

public class GetAllQueryHandlerTests
{
    private readonly Mock<IImovelRepository> _repoMock;
    private readonly GetAllQueryHandlerImovel _handler;

    private readonly List<Imovel> _ficticios =
    [
        new Imovel { Id = 1, Bloco = "A", Apartamento = "101", BoxGaragem = "A1" },
        new Imovel { Id = 2, Bloco = "B", Apartamento = "202", BoxGaragem = "B2" }
    ];

    public GetAllQueryHandlerTests()
    {
        _repoMock = new Mock<IImovelRepository>();
        _handler = new GetAllQueryHandlerImovel(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaDeImoveisMapeadaParaDto()
    {
        // Arrange
        long idprimeiroDto = 1;
        string blocoPrimeiroDto = "A";
        GetAllQueryImovel query = new();
        _repoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_ficticios);

        // Act
        Domain.Common.Result<IEnumerable<ImovelDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        List<ImovelDto> dtos = resultado.Dados.ToList();

        Assert.Equal(_ficticios.Count, dtos.Count);

        ImovelDto primeiroDto = dtos.First();

        Assert.IsType<ImovelDto>(primeiroDto);
        Assert.Equal(idprimeiroDto, primeiroDto.Id);
        Assert.Equal(blocoPrimeiroDto, primeiroDto.Bloco);

        _repoMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoHaImoveis_DeveRetornarListaVazia()
    {
        // Arrange
        GetAllQueryImovel query = new();
        _repoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Imovel>());

        // Act
        Domain.Common.Result<IEnumerable<ImovelDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Empty(resultado.Dados);

        _repoMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
}
