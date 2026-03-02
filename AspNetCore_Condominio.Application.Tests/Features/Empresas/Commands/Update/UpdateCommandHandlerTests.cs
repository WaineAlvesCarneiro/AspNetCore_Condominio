using AspNetCore_Condominio.Application.Features.Empresas.Commands.Update;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Empresas.Commands.Update;

public class UpdateCommandHandlerTests
{
    private readonly Mock<IEmpresaRepository> _repoMock;
    private readonly Mock<IAuthUserRepository> _authUserRepoMock;
    private readonly Mock<IMensageriaService> _mensageriaMock;
    private readonly UpdateCommandHandlerEmpresa _handler;
    private readonly Empresa _existente = new()
    {
        Id = 1,
        Ativo = TipoEmpresaAtivo.Ativo,
        RazaoSocial = "Razão Social",
        Fantasia = "Fantasia",
        Cnpj = "44764428000186",
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
        DataInclusao = DateTime.Now
    };

    public UpdateCommandHandlerTests()
    {
        _repoMock = new Mock<IEmpresaRepository>();
        _authUserRepoMock = new Mock<IAuthUserRepository>();
        _mensageriaMock = new Mock<IMensageriaService>();
        _handler = new UpdateCommandHandlerEmpresa(_repoMock.Object, _authUserRepoMock.Object, _mensageriaMock.Object);
    }

    [Fact]
    public async Task Handle_EmpresaExistenteEComandoValido_DeveAtualizarERetornarSucessoDto()
    {
        // Arrange
        UpdateCommandEmpresa command = new()
        {
            Id = 5,
            Ativo = TipoEmpresaAtivo.Ativo,
            RazaoSocial = "Razão Social Atualizada",
            Fantasia = "Fantasia Atualizada",
            Cnpj = "44764428000186",
            TipoDeCondominio = (TipoCondominio)1,
            Nome = "Responsável Atualizado",
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
            DataInclusao = DateTime.Now,
            DataAlteracao = DateTime.Now
        };

        // Act
        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync(_existente);
        Domain.Common.Result<DTOs.EmpresaDto> resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(command.RazaoSocial, resultado.Dados.RazaoSocial);
        Assert.Equal(command.Cnpj, resultado.Dados.Cnpj);

        _repoMock.Verify(repo => repo.UpdateAsync(
            It.Is<Empresa>(i => i.Id == 1 && i.RazaoSocial == command.RazaoSocial),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_EmpresaInexistente_DeveRetornarResultFailure()
    {
        string mensagemEsperada = "Empresa não encontrada.";
        UpdateCommandEmpresa command = new()
        {
            Id = 999,
            RazaoSocial = "Razão Social não existente",
            Fantasia = "Fantasia não existente",
            Cnpj = "44764428000100",
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
            DataInclusao = DateTime.Now
        };

        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync((Empresa)null!);
        Domain.Common.Result<DTOs.EmpresaDto> resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Contains(mensagemEsperada, resultado.Mensagem);
        Assert.Null(resultado.Dados);

        _repoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Empresa>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
