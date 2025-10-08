using AspNetCore_Condominio.Application.Features.Moradores.Commands.UpdateMorador;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Events;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;
using Moq;
using DateOnly = System.DateOnly;

namespace AspNetCore_Condominio.Application.Tests.Features.Moradores.Commands.UpdateMorador;

public class UpdateMoradorCommandHandlerTests
{
    private readonly Mock<IMoradorRepository> _moradorRepoMock;
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UpdateMoradorCommandHandler _handler;

    private const int MORADOR_ID_EXISTENTE = 5;
    private const int IMOVEL_ID_VALIDO = 1;
    private const int IMOVEL_ID_NOVO = 2;

    private readonly Imovel _imovelValido = new Imovel {
        Id = IMOVEL_ID_VALIDO,
        Bloco = "01",
        Apartamento = "101",
        BoxGaragem = "224"
    };
    private readonly Imovel _imovelNovo = new Imovel {
        Id = IMOVEL_ID_NOVO,
        Bloco = "09",
        Apartamento = "302",
        BoxGaragem = "134"
    };

    private readonly Morador _moradorExistente = new Morador
    {
        Id = MORADOR_ID_EXISTENTE,
        Nome = "Morador Antigo",
        Celular = "00000000000",
        Email = "antigo@old.com",
        IsProprietario = true,
        DataEntrada = new DateTime(2023, 1, 10, 10, 0, 0),
        DataInclusao = new DateTime(2023, 1, 10, 10, 0, 0),
        ImovelId = IMOVEL_ID_VALIDO,
        Imovel = new Imovel {
            Id = IMOVEL_ID_VALIDO,
            Bloco = "01",
            Apartamento = "101",
            BoxGaragem = "224"
        }
    };

    public UpdateMoradorCommandHandlerTests()
    {
        _moradorRepoMock = new Mock<IMoradorRepository>();
        _imovelRepoMock = new Mock<IImovelRepository>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new UpdateMoradorCommandHandler(
            _moradorRepoMock.Object,
            _imovelRepoMock.Object,
            _mediatorMock.Object
        );

        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(IMOVEL_ID_VALIDO)).ReturnsAsync(_imovelValido);
        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(IMOVEL_ID_NOVO)).ReturnsAsync(_imovelNovo);
        _moradorRepoMock.Setup(repo => repo.GetByIdAsync(MORADOR_ID_EXISTENTE)).ReturnsAsync(_moradorExistente);
        _moradorRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Morador>())).Returns(Task.CompletedTask);
    }

    private UpdateMoradorCommand GetValidCommand() => new()
    {
        Id = MORADOR_ID_EXISTENTE,
        Nome = "Morador Novo",
        Celular = "21991234567",
        Email = "novo@new.com",
        IsProprietario = false,
        DataEntrada = DateOnly.FromDateTime(_moradorExistente.DataEntrada),
        DataInclusao = DateOnly.FromDateTime(_moradorExistente.DataInclusao),
        DataSaida = null,
        DataAlteracao = DateOnly.FromDateTime(DateTime.Today),
        ImovelId = IMOVEL_ID_NOVO
    };

    [Fact]
    public async Task Handle_ComandoValido_DeveAtualizarDadosERetornarSucesso()
    {
        var command = GetValidCommand();
        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.Equal("Morador atualizado com sucesso.", resultado.Mensagem);
        Assert.Equal(command.Nome, resultado.Dados.Nome);
        Assert.Equal(IMOVEL_ID_NOVO, resultado.Dados.ImovelId); // Verifica a mudança de ID

        _moradorRepoMock.Verify(repo => repo.UpdateAsync(
            It.Is<Morador>(m => m.Nome == command.Nome && m.ImovelId == IMOVEL_ID_NOVO)
        ), Times.Once);
        _mediatorMock.Verify(m => m.Publish(
            It.Is<MoradorCriadoEvent>(e => e.IsCreate == false),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_MoradorInexistente_DeveRetornarFalha()
    {
        const int moradorIdInexistente = 99;
        var command = GetValidCommand();
        command.Id = moradorIdInexistente;
        _moradorRepoMock.Setup(repo => repo.GetByIdAsync(moradorIdInexistente)).ReturnsAsync((Morador)null!);
        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Contains("Morador não encontrado.", resultado.Mensagem);

        _moradorRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Morador>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ImovelParaAtualizacaoInexistente_DeveRetornarFalha()
    {
        const int imovelIdInexistente = 99;
        var command = GetValidCommand();
        command.ImovelId = imovelIdInexistente;
        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(imovelIdInexistente)).ReturnsAsync((Imovel)null!);
        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Contains("O imóvel informado não existe.", resultado.Mensagem);

        _moradorRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Morador>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ComDataSaida_DeveAtualizarDataSaidaComHora()
    {
        var dataSaida = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var command = GetValidCommand();
        command.DataSaida = dataSaida;
        Morador? moradorCapturado = null;
        _moradorRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Morador>()))
            .Callback<Morador>(m => moradorCapturado = m)
            .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(moradorCapturado);
        Assert.Equal(dataSaida.Year, moradorCapturado.DataSaida.Value.Year);
        Assert.Equal(dataSaida.Month, moradorCapturado.DataSaida.Value.Month);
        Assert.Equal(dataSaida.Day, moradorCapturado.DataSaida.Value.Day);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(moradorCapturado.DataAlteracao.Value));
    }
}
