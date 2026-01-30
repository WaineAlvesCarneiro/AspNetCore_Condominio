using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Auth.Queries.GetAll;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Auth.Querys.GetAll;

public class GetAllQueryHandlerTests
{
    private readonly Mock<IAuthUserRepository> _repoMock;
    private readonly GetAllQueryHandlerAuthUser _handler;

    private readonly List<AuthUser> _ficticios =
    [
        new AuthUser {
            Id = Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B"),
            EmpresaId = 1,
            UserName = "Admin",
            PasswordHash = "12345",
            Role = (TipoRole)1,
            DataInclusao = DateTime.UtcNow
        },
        new AuthUser {
            Id = Guid.Parse("FFFFF7AB-F0FD-F011-8550-A5241967915B"),
            EmpresaId = 1,
            UserName = "Sindico",
            PasswordHash = "12345",
            Role = (TipoRole)2,
            DataInclusao = DateTime.UtcNow
        },
    ];   

    public GetAllQueryHandlerTests()
    {
        _repoMock = new Mock<IAuthUserRepository>();
        _handler = new GetAllQueryHandlerAuthUser(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaDeAuthUsersMapeadaParaDto()
    {
        // Arrange
        Guid idprimeiroDto = Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B");
        string userNamePrimeiroDto = "Admin";
        GetAllQueryAuthUser query = new();
        _repoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_ficticios);

        // Act
        Domain.Common.Result<IEnumerable<AuthUserDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        List<AuthUserDto> dtos = resultado.Dados.ToList();

        Assert.Equal(_ficticios.Count, dtos.Count);

        AuthUserDto primeiroDto = dtos.First();

        Assert.IsType<AuthUserDto>(primeiroDto);
        Assert.Equal(idprimeiroDto, primeiroDto.Id);
        Assert.Equal(userNamePrimeiroDto, primeiroDto.UserName);

        _repoMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoHaAuthUsers_DeveRetornarListaVazia()
    {
        // Arrange
        GetAllQueryAuthUser query = new();
        _repoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<AuthUser>());

        // Act
        Domain.Common.Result<IEnumerable<AuthUserDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Empty(resultado.Dados);

        _repoMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
}
