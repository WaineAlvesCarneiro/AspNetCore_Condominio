using AspNetCore_Condominio.Application.Features.Empresas.Commands.Delete;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Empresas.Commands.Delete;

public class DeleteCommandHandlerTests
{
    private readonly Mock<IEmpresaRepository> _repoMock;
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly Mock<IAuthUserRepository> _authUserRepoMock;
    private readonly DeleteCommandHandlerEmpresa _handler;

    private const long UserEmpresaId = 1;
    private const int ID_EXISTENTE = 10;
    private const int ID_NAO_EXISTENTE = 999;
    private readonly Empresa _existente;

    public DeleteCommandHandlerTests()
    {
        _repoMock = new Mock<IEmpresaRepository>();
        _imovelRepoMock = new Mock<IImovelRepository>();
        _authUserRepoMock = new Mock<IAuthUserRepository>();

        _existente = new Empresa {
            Id = ID_EXISTENTE,
            RazaoSocial = "Razão Social Atualizada",
            Fantasia = "Fantasia Atualizada",
            Cnpj = "01.111.222/0001-02",
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
            DataInclusao = DateTime.Now
        };
        _handler = new DeleteCommandHandlerEmpresa(_repoMock.Object, _imovelRepoMock.Object, _authUserRepoMock.Object);
    }

    [Fact]
    public async Task Handle_EmpresaExisteESemImoveis_DeveDeletarERetornarSucesso()
    {
        // Arrange
        string mensagemSucesso = "Empresa deletada com sucesso.";
        DeleteCommandEmpresa command = new(ID_EXISTENTE);
        _imovelRepoMock.Setup(repo => repo.ExisteImovelVinculadoNaEmpresaAsync(command.Id)).ReturnsAsync(false);
        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync(_existente);

        // Act
        Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.Equal(mensagemSucesso, resultado.Mensagem);

        _repoMock.Verify(repo => repo.DeleteAsync(
            It.Is<Empresa>(i => i.Id == ID_EXISTENTE)
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_EmpresaComImoveisVinculados_DeveRetornarFalhaENaoDeletar()
    {
        // Arrange
        string mensagemFalha = "Não é possível excluir a empresa, pois tem imóvel vinculado.";
        DeleteCommandEmpresa command = new(ID_EXISTENTE);
        _imovelRepoMock.Setup(repo => repo.ExisteImovelVinculadoNaEmpresaAsync(command.Id)).ReturnsAsync(true);

        // Act
        Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal(mensagemFalha, resultado.Mensagem);

        _repoMock.Verify(repo => repo.GetByIdAsync(It.IsAny<long>()) , Times.Never);
        _repoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Empresa>()), Times.Never);
    }

    [Fact]
    public async Task Handle_EmpresaInexistenteESemImoveis_DeveRetornarFalha()
    {
        // Arrange
        string mensagemFalha = "Empresa não encontrada.";
        DeleteCommandEmpresa command = new(ID_NAO_EXISTENTE);
        _imovelRepoMock.Setup(repo => repo.ExisteImovelVinculadoNaEmpresaAsync(command.Id)).ReturnsAsync(false);
        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync((Empresa)null!);

        // Act
        Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal(mensagemFalha, resultado.Mensagem);

        _repoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Empresa>()), Times.Never);
    }
}