using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Empresas.Queries.GetAll;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Empresas.Querys.GetAll;

public class GetAllQueryHandlerTests
{
    private readonly Mock<IEmpresaRepository> _repoMock;
    private readonly GetAllQueryHandlerEmpresa _handler;

    private readonly List<Empresa> _ficticios =
    [
        new Empresa {
            Id = 1,
            RazaoSocial = "Razão Social",
            Fantasia = "Fantasia",
            Cnpj = "44764428000186",
            TipoDeCondominio = (TipoCondominio)1,
            Nome = "Responsável",
            Celular = "11999999999",
            Telefone = "1133333333",
            Email = "email@gmail.com",
            Senha = "SenhaForte123!",
            Host = "smtp.exemplo.com",
            Porta = 587,
            Cep = "01234567",
            Uf = "SP",
            Cidade = "São Paulo",
            Endereco = "Rua Exemplo, 123",
            Bairro = "Pq Amazônia",
            Complemento = "Complemento",
            DataInclusao = DateTime.Now
        }
    ];   

    public GetAllQueryHandlerTests()
    {
        _repoMock = new Mock<IEmpresaRepository>();
        _handler = new GetAllQueryHandlerEmpresa(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaDeEmpresasMapeadaParaDto()
    {
        // Arrange
        long idprimeiroDto = 1;
        string razaoSocialPrimeiroDto = "Razão Social";
        GetAllQueryEmpresa query = new();
        _repoMock.Setup(repo => repo.GetAllAsync(It.IsAny<long?>(),It.IsAny<CancellationToken>())).ReturnsAsync(_ficticios);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        List<EmpresaDto> dtos = resultado.Dados.ToList();

        Assert.Equal(_ficticios.Count, dtos.Count);

        EmpresaDto primeiroDto = dtos.First();

        Assert.IsType<EmpresaDto>(primeiroDto);
        Assert.Equal(idprimeiroDto, primeiroDto.Id);
        Assert.Equal(razaoSocialPrimeiroDto, primeiroDto.RazaoSocial);

        _repoMock.Verify(repo => repo.GetAllAsync(It.IsAny<long?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoHaEmpresas_DeveRetornarListaVazia()
    {
        // Arrange
        GetAllQueryEmpresa query = new();
        _repoMock.Setup(repo => repo.GetAllAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Empresa>());

        // Act
        Domain.Common.Result<IEnumerable<EmpresaDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Empty(resultado.Dados);
    }
}
