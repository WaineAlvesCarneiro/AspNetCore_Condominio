using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Auth.Commands.Create;

public class CreateCommandHandlerTests
{
    private readonly Mock<IAuthUserRepository> _repoMock;
    private readonly Mock<IMensageriaService> _mensageriaMock;
    private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
    private readonly Mock<ILogger<CreateCommandHandlerAuthUser>> _loggerMock;
    private readonly CreateCommandHandlerAuthUser _handler;

    public CreateCommandHandlerTests()
    {
        _repoMock = new Mock<IAuthUserRepository>();
        _mensageriaMock = new Mock<IMensageriaService>();
        _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
        _loggerMock = new Mock<ILogger<CreateCommandHandlerAuthUser>>();

        _handler = new CreateCommandHandlerAuthUser(_repoMock.Object, _mensageriaMock.Object, _emailTemplateServiceMock.Object, _loggerMock.Object);

        _repoMock.Setup(repo => repo.CreateAsync(It.IsAny<AuthUser>(), It.IsAny<CancellationToken>()))
            .Callback<AuthUser, CancellationToken>((authUser, token) => authUser.Id = Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B"))
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
            Email = "email@gmail.com",
            Role = (TipoRole)1,
            DataInclusao = DateTime.Now
        };

        // Act
        Domain.Common.Result<DTOs.AuthUserDto> resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(idGerado, resultado.Dados.Id);
        Assert.Equal(command.UserName, resultado.Dados.UserName);

        _repoMock.Verify(repo => repo.CreateAsync(
            It.Is<AuthUser>(i => i.UserName == command.UserName && i.Role == command.Role),
            It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}