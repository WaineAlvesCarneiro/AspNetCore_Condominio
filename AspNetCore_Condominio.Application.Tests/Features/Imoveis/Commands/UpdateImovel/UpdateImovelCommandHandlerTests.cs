using AspNetCore_Condominio.Application.Features.Imoveis.Commands.UpdateImovel;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.UpdateImovel;

public class UpdateImovelCommandHandlerTests
{
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly UpdateImovelCommandHandler _handler;

    private readonly Imovel _imovelExistente = new Imovel
    {
        Id = 5,
        Bloco = "Bloco Antigo",
        Apartamento = "100",
        BoxGaragem = "G1"
    };

    public UpdateImovelCommandHandlerTests()
    {
        _imovelRepoMock = new Mock<IImovelRepository>();
        _handler = new UpdateImovelCommandHandler(_imovelRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ImovelExistenteEComandoValido_DeveAtualizarERetornarSucessoDto()
    {
        var command = new UpdateImovelCommand
        {
            Id = 5,
            Bloco = "Bloco Novo",
            Apartamento = "101",
            BoxGaragem = "G2"
        };

        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync(_imovelExistente);
        var resultado = await _handler.Handle(command, CancellationToken.None);

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
        var command = new UpdateImovelCommand
        {
            Id = 999,
            Bloco = "Qualquer",
            Apartamento = "Qualquer",
            BoxGaragem = "Qualquer"
        };

        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync((Imovel)null!);
        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Contains("Imóvel não encontrado.", resultado.Mensagem);
        Assert.Null(resultado.Dados);

        _imovelRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Imovel>()), Times.Never);
    }
}
