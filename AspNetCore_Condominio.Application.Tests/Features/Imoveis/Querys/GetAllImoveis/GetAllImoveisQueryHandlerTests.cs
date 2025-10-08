using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAllImoveis;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Querys.GetAllImoveis;

public class GetAllImoveisQueryHandlerTests
{
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly GetAllImoveisQueryHandler _handler;

    private readonly List<Imovel> _imoveisFicticios = new List<Imovel>
    {
        new Imovel { Id = 1, Bloco = "A", Apartamento = "101", BoxGaragem = "A1" },
        new Imovel { Id = 2, Bloco = "B", Apartamento = "202", BoxGaragem = "B2" }
    };

    public GetAllImoveisQueryHandlerTests()
    {
        _imovelRepoMock = new Mock<IImovelRepository>();
        _handler = new GetAllImoveisQueryHandler(_imovelRepoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaDeImoveisMapeadaParaDto()
    {
        var query = new GetAllImoveisQuery();
        _imovelRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_imoveisFicticios);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        var dtos = resultado.Dados.ToList();

        Assert.Equal(_imoveisFicticios.Count, dtos.Count);
        
        var primeiroDto = dtos.First();

        Assert.IsType<ImovelDto>(primeiroDto);
        Assert.Equal(1, primeiroDto.Id);
        Assert.Equal("A", primeiroDto.Bloco);

        _imovelRepoMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoHaImoveis_DeveRetornarListaVazia()
    {
        var query = new GetAllImoveisQuery();
        _imovelRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Imovel>());
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Empty(resultado.Dados);

        _imovelRepoMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
}
