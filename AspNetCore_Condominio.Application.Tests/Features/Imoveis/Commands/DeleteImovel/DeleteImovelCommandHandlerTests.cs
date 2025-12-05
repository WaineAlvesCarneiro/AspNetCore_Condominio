using AspNetCore_Condominio.Application.Features.Imoveis.Commands.DeleteImovel;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.DeleteImovel;

public class DeleteImovelCommandHandlerTests
{
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly Mock<IMoradorRepository> _moradorRepoMock;
    private readonly DeleteImovelCommandHandler _handler;

    private const int IMOVEL_ID_EXISTENTE = 10;
    private const int IMOVEL_ID_INEXISTENTE = 999;
    private readonly Imovel _imovelExistente;

    public DeleteImovelCommandHandlerTests()
    {
        // Configura os mocks dos repositórios e inicializa o handler que será testado.
        // Isso permite que os testes verifiquem o comportamento do handler em diferentes cenários.
        // Os mocks simulam o comportamento dos repositórios sem depender de uma implementação real.
        _imovelRepoMock = new Mock<IImovelRepository>();
        _moradorRepoMock = new Mock<IMoradorRepository>();

        // Cria um imóvel existente para ser usado nos testes.
        // Isso facilita a configuração dos testes, permitindo que eles trabalhem com um imóvel conhecido.
        _imovelExistente = new Imovel { Id = IMOVEL_ID_EXISTENTE, Bloco = "A", Apartamento = "101", BoxGaragem = "224" };

        // Inicializa o handler com os repositórios mockados.
        _handler = new DeleteImovelCommandHandler(_imovelRepoMock.Object, _moradorRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ImovelExisteESemMoradores_DeveDeletarERetornarSucesso()
    {
        // Arrange
        string mensagemSucesso = "Imóvel deletado com sucesso.";
        DeleteImovelCommand command = new(IMOVEL_ID_EXISTENTE);

        // Act
        // Configura os mocks para simular que o imóvel existe e não possui moradores vinculados.
        // Isso prepara o cenário para testar a deleção do imóvel.
        // O método Setup define o comportamento esperado dos mocks quando os métodos são chamados.
        // ReturnsAsync especifica o valor que deve ser retornado de forma assíncrona.
        _moradorRepoMock.Setup(repo => repo.ExistsByImovelIdAsync(command.Id)).ReturnsAsync(false);
        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync(_imovelExistente);
        Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.Equal(mensagemSucesso, resultado.Mensagem);

        // Verifica se o método DeleteAsync foi chamado corretamente.
        // Isso confirma que o handler tentou deletar o imóvel existente.
        // O uso de It.Is<Imovel> permite verificar que o imóvel deletado é o correto.
        // Isso ajuda a garantir que o comportamento do handler está conforme o esperado.
        // O Times.Once assegura que o método foi chamado exatamente uma vez.
        _imovelRepoMock.Verify(repo => repo.DeleteAsync(
            It.Is<Imovel>(i => i.Id == IMOVEL_ID_EXISTENTE)
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ImovelComMoradoresVinculados_DeveRetornarFalhaENaoDeletar()
    {
        // Arrange
        string mensagemFalha = "Não é possível excluir o imóvel, pois existem moradores vinculados.";
        DeleteImovelCommand command = new(IMOVEL_ID_EXISTENTE);
        _moradorRepoMock.Setup(repo => repo.ExistsByImovelIdAsync(command.Id)).ReturnsAsync(true);

        // Act
        Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal(mensagemFalha, resultado.Mensagem);

        // Verifica que os métodos GetByIdAsync e DeleteAsync não foram chamados.
        // Isso confirma que o handler não tentou buscar ou deletar o imóvel quando havia moradores vinculados.
        // O Times.Never assegura que os métodos não foram chamados nenhuma vez.
        _imovelRepoMock.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
        // Verifica que o método DeleteAsync não foi chamado.
        // Isso confirma que o handler não tentou deletar o imóvel quando havia moradores vinculados.
        // O Times.Never assegura que o método não foi chamado nenhuma vez.
        _imovelRepoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Imovel>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ImovelInexistenteESemMoradores_DeveRetornarFalha()
    {
        // Arrange
        string mensagemFalha = "Imóvel não encontrado.";
        DeleteImovelCommand command = new(IMOVEL_ID_INEXISTENTE);
        _moradorRepoMock.Setup(repo => repo.ExistsByImovelIdAsync(command.Id)).ReturnsAsync(false);
        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync((Imovel)null!);

        // Act
        Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal(mensagemFalha, resultado.Mensagem);

        _imovelRepoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Imovel>()), Times.Never);
    }
}