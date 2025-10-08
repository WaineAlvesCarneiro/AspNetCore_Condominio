using AspNetCore_Condominio.Application.Features.Auth;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Auth;

public class AuthLoginQueryHandlerTests
{
    private readonly Mock<IAuthUserRepository> _authRepoMock;
    private readonly AuthLoginQueryHandler _handler;

    private const string USERNAME = "admin_teste";
    private const string SENHA_CORRETA = "pass123";
    private readonly AuthUser _user;

    public AuthLoginQueryHandlerTests()
    {
        _authRepoMock = new Mock<IAuthUserRepository>();

        var hashedPassword = PasswordHasher.HashPassword(SENHA_CORRETA);
        _user = new AuthUser
        {
            Id = Guid.NewGuid(),
            Username = USERNAME,
            PasswordHash = hashedPassword,
            Role = "Admin"
        };

        _handler = new AuthLoginQueryHandler(_authRepoMock.Object);
    }

    [Fact]
    public async Task Handle_CredenciaisCorretas_DeveRetornarUsuario()
    {
        var query = new AuthLoginQuery { Username = USERNAME, Password = SENHA_CORRETA };
        _authRepoMock.Setup(repo => repo.GetByUsernameAsync(USERNAME)).ReturnsAsync(_user);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(resultado);
        Assert.Equal(_user.Id, resultado.Id);

        _authRepoMock.Verify(repo => repo.GetByUsernameAsync(USERNAME), Times.Once);
    }

    [Fact]
    public async Task Handle_UsuarioInexistente_DeveRetornarNull()
    {
        var query = new AuthLoginQuery { Username = "nao_existe", Password = "any" };
        _authRepoMock.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((AuthUser)null!);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task Handle_SenhaIncorreta_DeveRetornarNull()
    {
        var query = new AuthLoginQuery { Username = USERNAME, Password = "senha_errada" };
        _authRepoMock.Setup(repo => repo.GetByUsernameAsync(USERNAME)).ReturnsAsync(_user);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(resultado);
    }
}