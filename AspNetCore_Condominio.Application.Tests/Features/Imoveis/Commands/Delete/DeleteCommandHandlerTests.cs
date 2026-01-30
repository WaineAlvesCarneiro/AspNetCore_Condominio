using AspNetCore_Condominio.Application.Features.Imoveis.Commands.Delete;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.Delete;

public class DeleteCommandHandlerTests
{
    private readonly Mock<IImovelRepository> _repoMock;
    private readonly Mock<IMoradorRepository> _moradorRepoMock;
    private readonly DeleteCommandHandlerImovel _handler;

    private const long UserEmpresaId = 1;
    private const int ID_EXISTENTE = 10;
    private const int ID_NAO_EXISTENTE = 999;
    private readonly Imovel _existente;

    public DeleteCommandHandlerTests()
    {
        _repoMock = new Mock<IImovelRepository>();
        _moradorRepoMock = new Mock<IMoradorRepository>();
        _existente = new Imovel { Id = ID_EXISTENTE, Bloco = "A", Apartamento = "101", BoxGaragem = "224", EmpresaId = UserEmpresaId };
        _handler = new DeleteCommandHandlerImovel(_repoMock.Object, _moradorRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ImovelExisteESemMoradores_DeveDeletarERetornarSucesso()
    {
        // Arrange
        string mensagemSucesso = "Imóvel deletado com sucesso.";
        DeleteCommandImovel command = new(ID_EXISTENTE, UserEmpresaId);
        _moradorRepoMock.Setup(repo => repo.ExisteMoradorVinculadoNoImovelAsync(command.Id)).ReturnsAsync(false);
        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id, UserEmpresaId)).ReturnsAsync(_existente);

        // Act
        Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.Equal(mensagemSucesso, resultado.Mensagem);

        _repoMock.Verify(repo => repo.DeleteAsync(
            It.Is<Imovel>(i => i.Id == ID_EXISTENTE)
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ImovelComMoradoresVinculados_DeveRetornarFalhaENaoDeletar()
    {
        // Arrange
        string mensagemFalha = "Não é possível excluir o imóvel, pois tem morador vinculado.";
        DeleteCommandImovel command = new(ID_EXISTENTE, UserEmpresaId);
        _moradorRepoMock.Setup(repo => repo.ExisteMoradorVinculadoNoImovelAsync(command.Id)).ReturnsAsync(true);

        // Act
        Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal(mensagemFalha, resultado.Mensagem);

        _repoMock.Verify(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<long>()) , Times.Never);
        _repoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Imovel>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ImovelInexistenteESemMoradores_DeveRetornarFalha()
    {
        // Arrange
        string mensagemFalha = "Imóvel não encontrado.";
        DeleteCommandImovel command = new(ID_NAO_EXISTENTE, UserEmpresaId);
        _moradorRepoMock.Setup(repo => repo.ExisteMoradorVinculadoNoImovelAsync(command.Id)).ReturnsAsync(false);
        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id, UserEmpresaId)).ReturnsAsync((Imovel)null!);

        // Act
        Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal(mensagemFalha, resultado.Mensagem);

        _repoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Imovel>()), Times.Never);
    }
}