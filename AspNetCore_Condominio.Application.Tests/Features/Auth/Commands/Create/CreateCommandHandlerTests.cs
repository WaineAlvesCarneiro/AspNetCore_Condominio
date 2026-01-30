using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Auth.Commands.Create;

public class CreateCommandHandlerTests
{
    private readonly Mock<IAuthUserRepository> _repoMock;
    private readonly CreateCommandHandlerAuthUser _handler;

    public CreateCommandHandlerTests()
    {
        _repoMock = new Mock<IAuthUserRepository>();
        _handler = new CreateCommandHandlerAuthUser(_repoMock.Object);
        _repoMock.Setup(repo => repo.CreateAsync(It.IsAny<AuthUser>()))
            .Callback<AuthUser>(authUser => authUser.Id = Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B"))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Handle_ComandoValido_DeveChamarCreateAsyncERetornarSucessoDto()
    {
        // Arrange
        Guid idGerado = Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B");
        CreateCommandAuthUser command = new()
        {
            EmpresaId = 1,
            UserName = "Admin",
            PasswordHash = "12345",
            Role = (TipoRole)1,
            DataInclusao = DateTime.UtcNow
        };

        // Act
        Domain.Common.Result<DTOs.AuthUserDto> resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(idGerado, resultado.Dados.Id);
        Assert.Equal(command.UserName, resultado.Dados.UserName);

        _repoMock.Verify(repo => repo.CreateAsync(
            It.Is<AuthUser>(
                i => i.UserName == command.UserName && i.Role == command.Role
            )),
            Times.Once
        );
    }
}