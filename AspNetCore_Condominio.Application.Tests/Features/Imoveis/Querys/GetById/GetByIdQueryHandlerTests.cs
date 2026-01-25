using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetById;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Querys.GetById;

public class GetByIdQueryHandlerTests
{
    private readonly Mock<IImovelRepository> _repoMock;
    private readonly GetByIdQueryHandlerImovel _handler;

    private const int ID_EXISTENTE = 7;
    private const int ID_NAO_EXISTENTE = 99;

    private readonly Imovel _existente = new()
    {
        Id = ID_EXISTENTE,
        Bloco = "Bloco X",
        Apartamento = "305",
        BoxGaragem = "G7"
    };

    public GetByIdQueryHandlerTests()
    {
        _repoMock = new Mock<IImovelRepository>();
        _handler = new GetByIdQueryHandlerImovel(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ImovelExistente_DeveRetornarSucessoComImovelDto()
    {
        var query = new GetByIdQueryImovel(ID_EXISTENTE);
        _repoMock.Setup(repo => repo.GetByIdAsync(ID_EXISTENTE)).ReturnsAsync(_existente);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        var dto = resultado.Dados;

        Assert.IsType<ImovelDto>(dto);
        Assert.Equal(ID_EXISTENTE, dto.Id);
        Assert.Equal(_existente.Bloco, dto.Bloco);

        _repoMock.Verify(repo => repo.GetByIdAsync(ID_EXISTENTE), Times.Once);
    }

    [Fact]
    public async Task Handle_ImovelInexistente_DeveRetornarFailure()
    {
        var query = new GetByIdQueryImovel(ID_NAO_EXISTENTE);
        _repoMock.Setup(repo => repo.GetByIdAsync(ID_NAO_EXISTENTE)).ReturnsAsync((Imovel)null!);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Imóvel não encontrado.", resultado.Mensagem);
        Assert.Null(resultado.Dados);

        _repoMock.Verify(repo => repo.GetByIdAsync(ID_NAO_EXISTENTE), Times.Once);
    }
}