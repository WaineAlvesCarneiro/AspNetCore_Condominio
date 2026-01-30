using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Empresas.Queries.GetAllPaged;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Empresas.Querys.GetAllPaged;

public class GetAllPagedQueryHandlerTests
{
    private readonly Mock<IEmpresaRepository> _repoMock;
    private readonly GetAllPagedQueryHandlerEmpresa _handler;

    private readonly List<Empresa> _pagina1 =
    [
        new Empresa {
            Id = 1,
            RazaoSocial = "Razão Social",
            Fantasia = "Fantasia",
            Cnpj = "01.111.222/0001-02",
            TipoDeCondominio = (TipoCondominio)1,
            Nome = "Responsável",
            Celular = "(11) 99999-9999",
            Telefone = "(11) 3333-3333",
            Email = "email@gmail.com",
            Senha = "SenhaForte123!",
            Host = "smtp.exemplo.com",
            Porta = 587,
            Cep = "01234-567",
            Uf = "SP",
            Cidade = "São Paulo",
            Endereco = "Rua Exemplo, 123",
            Bairro = "Pq Amazônia",
            Complemento = "Complemento",
            DataInclusao = DateTime.UtcNow
        },
        new Empresa {
            Id = 2,
            RazaoSocial = "Razão Social 2",
            Fantasia = "Fantasia 2",
            Cnpj = "01.111.222/0001-02",
            TipoDeCondominio = (TipoCondominio)1,
            Nome = "Responsável",
            Celular = "(11) 99999-9999",
            Telefone = "(11) 3333-3333",
            Email = "email@gmail.com",
            Senha = "SenhaForte123!",
            Host = "smtp.exemplo.com",
            Porta = 587,
            Cep = "01234-567",
            Uf = "SP",
            Cidade = "São Paulo",
            Endereco = "Rua Exemplo, 123",
            Bairro = "Pq Amazônia",
            Complemento = "Complemento",
            DataInclusao = DateTime.UtcNow
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
        _repoMock = new Mock<IEmpresaRepository>();
        _handler = new GetAllPagedQueryHandlerEmpresa(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarPagedResultComDadosCorretos()
    {
        // Arrange
        string expectedFirstRazaoSocial = "Razão Social";
        GetAllPagedQueryEmpresa query = new(
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
        Result<PagedResult<EmpresaDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        PagedResult<EmpresaDto> pagedResult = resultado.Dados;

        Assert.Equal(TOTAL_REGISTROS, pagedResult.TotalCount);
        Assert.Equal(Page, pagedResult.PageIndex);
        Assert.Equal(PageSize, pagedResult.PageSize);
        Assert.Equal(_pagina1.Count, pagedResult.Items.Count());
        Assert.Equal(expectedFirstRazaoSocial, pagedResult.Items.First().RazaoSocial);
        Assert.IsType<PagedResult<EmpresaDto>>(pagedResult);

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
        GetAllPagedQueryEmpresa query = new();
        _repoMock.Setup(repo => repo.GetAllPagedAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((new List<Empresa>(), totalZero));
        Result<PagedResult<EmpresaDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(totalZero, resultado.Dados.TotalCount);
        Assert.Empty(resultado.Dados.Items);
    }
}
