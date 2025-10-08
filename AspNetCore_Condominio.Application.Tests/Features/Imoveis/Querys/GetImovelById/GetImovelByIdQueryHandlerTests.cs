using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetImovelById;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Querys.GetImovelById;

public class GetImovelByIdQueryHandlerTests
{
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly GetImovelByIdQueryHandler _handler;

    private const int IMOVEL_ID_EXISTENTE = 7;
    private const int IMOVEL_ID_INEXISTENTE = 99;

    private readonly Imovel _imovelExistente = new Imovel
    {
        Id = IMOVEL_ID_EXISTENTE,
        Bloco = "Bloco X",
        Apartamento = "305",
        BoxGaragem = "G7"
    };

    public GetImovelByIdQueryHandlerTests()
    {
        _imovelRepoMock = new Mock<IImovelRepository>();
        _handler = new GetImovelByIdQueryHandler(_imovelRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ImovelExistente_DeveRetornarSucessoComImovelDto()
    {
        var query = new GetImovelByIdQuery(IMOVEL_ID_EXISTENTE);
        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(IMOVEL_ID_EXISTENTE)).ReturnsAsync(_imovelExistente);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        var dto = resultado.Dados;

        Assert.IsType<ImovelDto>(dto);
        Assert.Equal(IMOVEL_ID_EXISTENTE, dto.Id);
        Assert.Equal(_imovelExistente.Bloco, dto.Bloco);

        _imovelRepoMock.Verify(repo => repo.GetByIdAsync(IMOVEL_ID_EXISTENTE), Times.Once);
    }

    [Fact]
    public async Task Handle_ImovelInexistente_DeveRetornarFailure()
    {
        var query = new GetImovelByIdQuery(IMOVEL_ID_INEXISTENTE);
        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(IMOVEL_ID_INEXISTENTE)).ReturnsAsync((Imovel)null!);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Imóvel não encontrado.", resultado.Mensagem);
        Assert.Null(resultado.Dados);

        _imovelRepoMock.Verify(repo => repo.GetByIdAsync(IMOVEL_ID_INEXISTENTE), Times.Once);
    }
}