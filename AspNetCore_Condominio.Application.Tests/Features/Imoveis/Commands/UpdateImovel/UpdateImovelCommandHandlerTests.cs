using AspNetCore_Condominio.Application.Features.Imoveis.Commands.UpdateImovel;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.UpdateImovel;

public class UpdateImovelCommandHandlerTests
{
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly UpdateImovelCommandHandler _handler;

    private readonly Imovel _imovelExistente = new()
    {
        Id = 5,
        Bloco = "Bloco Antigo",
        Apartamento = "100",
        BoxGaragem = "G1"
    };

    public UpdateImovelCommandHandlerTests()
    {
        _imovelRepoMock = new Mock<IImovelRepository>();

        // SUT
        // System Under Test
        // Objeto sendo testado
        // No caso, o handler do comando de atualização de imóvel
        // Estamos injetando o mock do repositório de imóveis para isolar o teste e controlar o comportamento do repositório
        _handler = new UpdateImovelCommandHandler(_imovelRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ImovelExistenteEComandoValido_DeveAtualizarERetornarSucessoDto()
    {
        // Arrange
        UpdateImovelCommand command = new()
        {
            Id = 5,
            Bloco = "Bloco Novo",
            Apartamento = "101",
            BoxGaragem = "G2"
        };

        // Act
        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync(_imovelExistente);
        Domain.Common.Result<DTOs.ImovelDto> resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(command.Bloco, resultado.Dados.Bloco);
        Assert.Equal(command.Apartamento, resultado.Dados.Apartamento);

        _imovelRepoMock.Verify(repo => repo.UpdateAsync(
            It.Is<Imovel>(i => i.Id == 5 && i.Bloco == command.Bloco)
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ImovelInexistente_DeveRetornarResultFailure()
    {
        string mensagemEsperada = "Imóvel não encontrado.";
        UpdateImovelCommand command = new()
        {
            Id = 999,
            Bloco = "Qualquer",
            Apartamento = "Qualquer",
            BoxGaragem = "Qualquer"
        };

        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync((Imovel)null!);
        Domain.Common.Result<DTOs.ImovelDto> resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Contains(mensagemEsperada, resultado.Mensagem);
        Assert.Null(resultado.Dados);

        _imovelRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Imovel>()), Times.Never);
    }
}
