using AspNetCore_Condominio.Application.Features.Auth.Commands.Delete;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Auth.Commands.Delete;

public class DeleteCommandHandlerTests
{
    private readonly Mock<IAuthUserRepository> _repoMock;
    private readonly DeleteCommandHandlerAuthUser _handler;

    private readonly AuthUser _existente;

    public DeleteCommandHandlerTests()
    {
        _repoMock = new Mock<IAuthUserRepository>();
        _existente = new AuthUser {
            Id = Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B"),
            EmpresaId = 1,
            UserName = "Admin",
            PasswordHash = "12345",
            Role = (TipoRole)1,
            DataInclusao = DateTime.UtcNow
        };
        _handler = new DeleteCommandHandlerAuthUser(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_AuthUserExisteESemImoveis_DeveDeletarERetornarSucesso()
    {
        // Arrange
        string mensagemSucesso = "Usuário deletado com sucesso.";
        DeleteCommandAuthUser command = new(Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B"));
        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync(_existente);

        // Act
        Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.Equal(mensagemSucesso, resultado.Mensagem);

        _repoMock.Verify(repo => repo.DeleteAsync(
            It.Is<AuthUser>(i => i.Id == Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B"))
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_AuthUserInexistenteESemImoveis_DeveRetornarFalha()
    {
        // Arrange
        string mensagemFalha = "Usuário não encontrado.";
        DeleteCommandAuthUser command = new(Guid.Parse("FFFF57AB-F0FD-F011-8550-A5241967915B"));
        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync((AuthUser)null!);

        // Act
        Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal(mensagemFalha, resultado.Mensagem);

        _repoMock.Verify(repo => repo.DeleteAsync(It.IsAny<AuthUser>()), Times.Never);
    }
}