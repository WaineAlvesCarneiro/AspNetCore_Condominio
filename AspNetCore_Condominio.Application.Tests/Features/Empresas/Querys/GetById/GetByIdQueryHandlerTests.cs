using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Empresas.Queries.GetById;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Empresas.Querys.GetById;

public class GetByIdQueryHandlerTests
{
    private readonly Mock<IEmpresaRepository> _repoMock;
    private readonly GetByIdQueryHandlerEmpresa _handler;

    private const int ID_EXISTENTE = 7;
    private const int ID_NAO_EXISTENTE = 99;

    private readonly Empresa _existente = new()
    {
        Id = ID_EXISTENTE,
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
    };

    public GetByIdQueryHandlerTests()
    {
        _repoMock = new Mock<IEmpresaRepository>();
        _handler = new GetByIdQueryHandlerEmpresa(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_EmpresaExistente_DeveRetornarSucessoComEmpresaDto()
    {
        var query = new GetByIdQueryEmpresa(ID_EXISTENTE);
        _repoMock.Setup(repo => repo.GetByIdAsync(ID_EXISTENTE)).ReturnsAsync(_existente);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        var dto = resultado.Dados;

        Assert.IsType<EmpresaDto>(dto);
        Assert.Equal(ID_EXISTENTE, dto.Id);
        Assert.Equal(_existente.RazaoSocial, dto.RazaoSocial);

        _repoMock.Verify(repo => repo.GetByIdAsync(ID_EXISTENTE), Times.Once);
    }

    [Fact]
    public async Task Handle_EmpresaInexistente_DeveRetornarFailure()
    {
        var query = new GetByIdQueryEmpresa(ID_NAO_EXISTENTE);
        _repoMock.Setup(repo => repo.GetByIdAsync(ID_NAO_EXISTENTE)).ReturnsAsync((Empresa)null!);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Empresa não encontrada.", resultado.Mensagem);
        Assert.Null(resultado.Dados);

        _repoMock.Verify(repo => repo.GetByIdAsync(ID_NAO_EXISTENTE), Times.Once);
    }
}