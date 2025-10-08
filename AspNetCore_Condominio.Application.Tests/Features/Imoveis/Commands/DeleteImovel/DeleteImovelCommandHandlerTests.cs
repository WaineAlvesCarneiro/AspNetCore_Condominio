using AspNetCore_Condominio.Application.Features.Imoveis.Commands.DeleteImovel;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.DeleteImovel;

public class DeleteImovelCommandHandlerTests
{
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly Mock<IMoradorRepository> _moradorRepoMock;
    private readonly DeleteImovelCommandHandler _handler;

    private const int IMOVEL_ID_EXISTENTE = 10;
    private const int IMOVEL_ID_INEXISTENTE = 999;
    private readonly Imovel _imovelExistente;

    public DeleteImovelCommandHandlerTests()
    {
        _imovelRepoMock = new Mock<IImovelRepository>();
        _moradorRepoMock = new Mock<IMoradorRepository>();
        _imovelExistente = new Imovel { Id = IMOVEL_ID_EXISTENTE, Bloco = "A", Apartamento = "101", BoxGaragem = "224" };
        _handler = new DeleteImovelCommandHandler(_imovelRepoMock.Object, _moradorRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ImovelExisteESemMoradores_DeveDeletarERetornarSucesso()
    {
        var command = new DeleteImovelCommand(IMOVEL_ID_EXISTENTE);
        _moradorRepoMock.Setup(repo => repo.ExistsByImovelIdAsync(command.Id)).ReturnsAsync(false);
        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync(_imovelExistente);
        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.Equal("Imóvel deletado com sucesso.", resultado.Mensagem);

        _imovelRepoMock.Verify(repo => repo.DeleteAsync(
            It.Is<Imovel>(i => i.Id == IMOVEL_ID_EXISTENTE)
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ImovelComMoradoresVinculados_DeveRetornarFalhaENaoDeletar()
    {
        var command = new DeleteImovelCommand(IMOVEL_ID_EXISTENTE);
        _moradorRepoMock.Setup(repo => repo.ExistsByImovelIdAsync(command.Id)).ReturnsAsync(true);
        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Não é possível excluir o imóvel, pois existem moradores vinculados.", resultado.Mensagem);

        _imovelRepoMock.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _imovelRepoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Imovel>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ImovelInexistenteESemMoradores_DeveRetornarFalha()
    {
        var command = new DeleteImovelCommand(IMOVEL_ID_INEXISTENTE);
        _moradorRepoMock.Setup(repo => repo.ExistsByImovelIdAsync(command.Id)).ReturnsAsync(false);
        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync((Imovel)null!);
        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Imóvel não encontrado.", resultado.Mensagem);

        _imovelRepoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Imovel>()), Times.Never);
    }
}