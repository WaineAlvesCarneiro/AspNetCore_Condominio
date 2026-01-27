using AspNetCore_Condominio.Application.Features.Moradores.Commands.Delete;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Moradores.Commands.Delete;

public class DeleteCommandHandlerTests
{
    private readonly Mock<IMoradorRepository> _repoMock;
    private readonly DeleteCommandHandlerMorador _handler;

    private const long UserEmpresaId = 1;

    private readonly Morador _existente = new Morador
    {
        Id = 42,
        Nome = "Teste Delete",
        Celular = "99999999999",
        Email = "delete@teste.com",
        ImovelId = 1,
        EmpresaId = UserEmpresaId
    };

    public DeleteCommandHandlerTests()
    {
        _repoMock = new Mock<IMoradorRepository>();
        _handler = new DeleteCommandHandlerMorador(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_MoradorExistente_DeveChamarDeleteAsyncERetornarSucesso()
    {
        var command = new DeleteCommandMorador(_existente.Id, UserEmpresaId);
        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id, UserEmpresaId)).ReturnsAsync(_existente);
        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.Equal("Morador deletado com sucesso.", resultado.Mensagem);

        _repoMock.Verify(repo => repo.DeleteAsync(
            It.Is<Morador>(m => m.Id == _existente.Id)
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_MoradorInexistente_DeveRetornarFailure()
    {
        const long idInexistente = 999;
        var command = new DeleteCommandMorador(idInexistente, UserEmpresaId);
        _repoMock.Setup(repo => repo.GetByIdAsync(idInexistente, UserEmpresaId)).ReturnsAsync((Morador)null!);
        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Morador não encontrado.", resultado.Mensagem);

        _repoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Morador>()), Times.Never);
    }
}
