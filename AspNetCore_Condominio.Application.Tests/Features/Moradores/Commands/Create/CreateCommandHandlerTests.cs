using AspNetCore_Condominio.Application.Features.Moradores.Commands.Create;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Events;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;
using Moq;
using DateTime = System.DateTime;

namespace AspNetCore_Condominio.Application.Tests.Features.Moradores.Commands.Create;

public class CreateCommandHandlerTests
{
    private readonly Mock<IMoradorRepository> _repoMock;
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CreateCommandHandlerMorador _handler;

    private const int IMOVEL_ID_VALIDO = 1;
    private const int EMPRESA_ID_VALIDO = 1;
    private readonly Imovel _imovelValido = new Imovel {
        Id = IMOVEL_ID_VALIDO,
        Bloco = "01",
        Apartamento = "101",
        BoxGaragem = "224",
        EmpresaId = EMPRESA_ID_VALIDO
    };

    public CreateCommandHandlerTests()
    {
        _repoMock = new Mock<IMoradorRepository>();
        _imovelRepoMock = new Mock<IImovelRepository>();
        _mediatorMock = new Mock<IMediator>();
        _handler = new CreateCommandHandlerMorador(
            _repoMock.Object,
            _imovelRepoMock.Object,
            _mediatorMock.Object
        );
        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(IMOVEL_ID_VALIDO)).ReturnsAsync(_imovelValido);
        _repoMock.Setup(repo => repo.CreateAsync(It.IsAny<Morador>()))
            .Callback<Morador>(morador => morador.Id = 5)
            .Returns(Task.CompletedTask);
    }

    private CreateCommandMorador GetValidCommand() => new()
    {
        Nome = "Maria Teste",
        Celular = "21991234567",
        Email = "maria@teste.com",
        IsProprietario = false,
        DataEntrada = DateOnly.FromDateTime(DateTime.Now),
        DataInclusao = DateTime.Now,
        ImovelId = IMOVEL_ID_VALIDO,
        EmpresaId = EMPRESA_ID_VALIDO
    };

    [Fact]
    public async Task Handle_ComandoValidoEImovelExiste_DeveCriarMoradorEPublicarEvento()
    {
        var command = GetValidCommand();
        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(5, resultado.Dados.Id);

        _repoMock.Verify(repo => repo.CreateAsync(
            It.Is<Morador>(m => m.Nome == command.Nome)
        ), Times.Once);
    }


    [Fact]
    public async Task Handle_ImovelInexistente_DeveRetornarFalhaENaoCriarMorador()
    {
        const long ImovelIdInexistente = 99;
        var command = GetValidCommand();
        command.ImovelId = ImovelIdInexistente;

        _imovelRepoMock.Setup(repo => repo.GetByIdAsync(ImovelIdInexistente)).ReturnsAsync((Imovel)null!);
        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Contains("O imóvel informado não existe.", resultado.Mensagem);

        _repoMock.Verify(repo => repo.CreateAsync(It.IsAny<Morador>()), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<CriadoEventEmail<Morador>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveMapearDataEntradaEInclusaoCorretamente()
    {
        var dataEntrada = new DateTime(2023, 10, 5);
        var command = GetValidCommand();
        command.DataEntrada = DateOnly.FromDateTime(dataEntrada);

        Morador? moradorCapturado = null;
        _repoMock.Setup(repo => repo.CreateAsync(It.IsAny<Morador>()))
            .Callback<Morador>(m => moradorCapturado = m)
            .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(moradorCapturado);
        Assert.Equal(dataEntrada.Year, moradorCapturado.DataEntrada.Year);
        Assert.Equal(dataEntrada.Month, moradorCapturado.DataEntrada.Month);
        Assert.Equal(dataEntrada.Day, moradorCapturado.DataEntrada.Day);
        Assert.Equal(DateTime.Now.Year, moradorCapturado.DataInclusao.Year);
    }
}