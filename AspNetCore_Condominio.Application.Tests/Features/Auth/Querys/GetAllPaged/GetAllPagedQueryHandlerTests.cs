using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Auth.Queries.GetAllPaged;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.AuthUsers.Querys.GetAllPaged;

public class GetAllPagedQueryHandlerTests
{
    private readonly Mock<IAuthUserRepository> _repoMock;
    private readonly GetAllPagedQueryHandlerAuthUser _handler;

    private readonly List<AuthUser> _pagina1 =
    [
        new AuthUser {
            Id = Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B"),
            EmpresaId = 1,
            UserName = "Admin",
            Email = "email@gmail.com",
            PasswordHash = "12345",
            Role = (TipoRole)1,
            DataInclusao = DateTime.Now
        },
        new AuthUser {
            Id = Guid.Parse("FFFFF7AB-F0FD-F011-8550-A5241967915B"),
            EmpresaId = 1,
            UserName = "Sindico",
            Email = "email@gmail.com",
            PasswordHash = "12345",
            Role = (TipoRole)2,
            DataInclusao = DateTime.Now
        },
    ];

    private const int Page = 1;
    private const int PageSize = 10;
    private const string? SortBy = "Id";
    private const string? SortDescending = "ASC";
    private const string? SearchTerm = null;

    private const int TOTAL_REGISTROS = 2;

    public GetAllPagedQueryHandlerTests()
    {
        _repoMock = new Mock<IAuthUserRepository>();
        _handler = new GetAllPagedQueryHandlerAuthUser(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarPagedResultComDadosCorretos()
    {
        // Arrange
        string expectedFirstUserName = "Admin";
        GetAllPagedQueryAuthUser query = new(
            Page: Page,
            PageSize: PageSize,
            SortBy: SortBy,
            SortDescending: SortDescending!
        );

        _repoMock.Setup(repo => repo.GetAllPagedAsync(
            Page,
            PageSize,
            SortBy,
            SortDescending,
            SearchTerm))
            .ReturnsAsync((_pagina1, TOTAL_REGISTROS));

        // Act
        Result<PagedResult<AuthUserDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        PagedResult<AuthUserDto> pagedResult = resultado.Dados;

        Assert.Equal(TOTAL_REGISTROS, pagedResult.TotalCount);
        Assert.Equal(Page, pagedResult.PageIndex);
        Assert.Equal(PageSize, pagedResult.PageSize);
        Assert.Equal(_pagina1.Count, pagedResult.Items.Count());
        Assert.Equal(expectedFirstUserName, pagedResult.Items.First().UserName);
        Assert.IsType<PagedResult<AuthUserDto>>(pagedResult);

        _repoMock.Verify(repo => repo.GetAllPagedAsync(
            Page,
            PageSize,
            SortBy,
            SortDescending,
            SearchTerm), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoRetornaListaVazia_DeveRetornarPagedResultVazio()
    {
        const int totalZero = 0;
        GetAllPagedQueryAuthUser query = new();
        _repoMock.Setup(repo => repo.GetAllPagedAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((new List<AuthUser>(), totalZero));
        Result<PagedResult<AuthUserDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(totalZero, resultado.Dados.TotalCount);
        Assert.Empty(resultado.Dados.Items);
    }
}
