using AspNetCore_Condominio.Application.Features.Auth.Commands.Update;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Auth.Commands.Update;

public class UpdateCommandHandlerTests
{
    private readonly Mock<IAuthUserRepository> _repoMock;
    private readonly UpdateCommandHandlerAuthUser _handler;
    private readonly AuthUser _existente = new()
    {
        Id = Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B"),
        EmpresaId = 1,
        UserName = "Admin",
        PasswordHash = "12345",
        Role = (TipoRole)1,
        DataInclusao = DateTime.UtcNow
    };

    public UpdateCommandHandlerTests()
    {
        _repoMock = new Mock<IAuthUserRepository>();
        _handler = new UpdateCommandHandlerAuthUser(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_AuthUserExistenteEComandoValido_DeveAtualizarERetornarSucessoDto()
    {
        // Arrange
        UpdateCommandAuthUser command = new()
        {
            Id = Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B"),
            EmpresaId = 1,
            UserName = "Admin Alterado",
            PasswordHash = "12345",
            Role = (TipoRole)1,
            DataInclusao = DateTime.UtcNow
        };

        // Act
        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync(_existente);
        Domain.Common.Result<DTOs.AuthUserDto> resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(command.UserName, resultado.Dados.UserName);

        _repoMock.Verify(repo => repo.UpdateAsync(
            It.Is<AuthUser>(i => i.Id == Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B") && i.UserName == command.UserName)
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_AuthUserInexistente_DeveRetornarResultFailure()
    {
        string mensagemEsperada = "Usuário não encontrado.";
        UpdateCommandAuthUser command = new()
        {
            Id = Guid.Parse("FFFF57AB-F0FD-F011-8550-A5241967915B"),
            EmpresaId = 1,
            UserName = "Admin",
            PasswordHash = "12345",
            Role = (TipoRole)1,
            DataInclusao = DateTime.UtcNow
        };

        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync((AuthUser)null!);
        Domain.Common.Result<DTOs.AuthUserDto> resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Contains(mensagemEsperada, resultado.Mensagem);
        Assert.Null(resultado.Dados);

        _repoMock.Verify(repo => repo.UpdateAsync(It.IsAny<AuthUser>()), Times.Never);
    }
}
