using AspNetCore_Condominio.Application.Features.Moradores.Commands.DeleteMorador;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Moradores.Commands.DeleteMorador;

public class DeleteMoradorCommandHandlerTests
{
    private readonly Mock<IMoradorRepository> _moradorRepoMock;
    private readonly DeleteMoradorCommandHandler _handler;

    private readonly Morador _moradorExistente = new Morador
    {
        Id = 42,
        Nome = "Teste Delete",
        Celular = "99999999999",
        Email = "delete@teste.com",
        ImovelId = 1
    };

    public DeleteMoradorCommandHandlerTests()
    {
        _moradorRepoMock = new Mock<IMoradorRepository>();
        _handler = new DeleteMoradorCommandHandler(_moradorRepoMock.Object);
    }

    [Fact]
    public async Task Handle_MoradorExistente_DeveChamarDeleteAsyncERetornarSucesso()
    {
        var command = new DeleteMoradorCommand(_moradorExistente.Id);
        _moradorRepoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync(_moradorExistente);
        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.Equal("Morador deletado com sucesso.", resultado.Mensagem);

        _moradorRepoMock.Verify(repo => repo.DeleteAsync(
            It.Is<Morador>(m => m.Id == _moradorExistente.Id)
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_MoradorInexistente_DeveRetornarFailure()
    {
        const int idInexistente = 999;
        var command = new DeleteMoradorCommand(idInexistente);
        _moradorRepoMock.Setup(repo => repo.GetByIdAsync(idInexistente)).ReturnsAsync((Morador)null!);
        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Morador não encontrado.", resultado.Mensagem);

        _moradorRepoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Morador>()), Times.Never);
    }
}
