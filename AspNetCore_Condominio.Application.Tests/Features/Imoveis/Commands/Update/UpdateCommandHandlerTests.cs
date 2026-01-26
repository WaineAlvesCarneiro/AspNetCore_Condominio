using AspNetCore_Condominio.Application.Features.Imoveis.Commands.Update;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.Update;

public class UpdateCommandHandlerTests
{
    private readonly Mock<IImovelRepository> _repoMock;
    private readonly UpdateCommandHandlerImovel _handler;

    private const long UserEmpresaId = 1;

    private readonly Imovel _existente = new()
    {
        Id = 5,
        Bloco = "Bloco Antigo",
        Apartamento = "100",
        BoxGaragem = "G1"
    };

    public UpdateCommandHandlerTests()
    {
        _repoMock = new Mock<IImovelRepository>();
        _handler = new UpdateCommandHandlerImovel(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ImovelExistenteEComandoValido_DeveAtualizarERetornarSucessoDto()
    {
        // Arrange
        UpdateCommandImovel command = new()
        {
            Id = 5,
            Bloco = "Bloco Novo",
            Apartamento = "101",
            BoxGaragem = "G2"
        };

        // Act
        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id, UserEmpresaId)).ReturnsAsync(_existente);
        Domain.Common.Result<DTOs.ImovelDto> resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(command.Bloco, resultado.Dados.Bloco);
        Assert.Equal(command.Apartamento, resultado.Dados.Apartamento);

        _repoMock.Verify(repo => repo.UpdateAsync(
            It.Is<Imovel>(i => i.Id == 5 && i.Bloco == command.Bloco)
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ImovelInexistente_DeveRetornarResultFailure()
    {
        string mensagemEsperada = "Imóvel não encontrado.";
        UpdateCommandImovel command = new()
        {
            Id = 999,
            Bloco = "Qualquer",
            Apartamento = "Qualquer",
            BoxGaragem = "Qualquer"
        };

        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id, UserEmpresaId)).ReturnsAsync((Imovel)null!);
        Domain.Common.Result<DTOs.ImovelDto> resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Contains(mensagemEsperada, resultado.Mensagem);
        Assert.Null(resultado.Dados);

        _repoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Imovel>()), Times.Never);
    }
}
