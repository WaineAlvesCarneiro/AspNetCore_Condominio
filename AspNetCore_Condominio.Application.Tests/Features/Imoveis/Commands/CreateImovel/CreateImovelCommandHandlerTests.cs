using AspNetCore_Condominio.Application.Features.Imoveis.Commands.CreateImovel;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.CreateImovel;

public class CreateImovelCommandHandlerTests
{
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly CreateImovelCommandHandler _handler;

    public CreateImovelCommandHandlerTests()
    {
        _imovelRepoMock = new Mock<IImovelRepository>();
        _handler = new CreateImovelCommandHandler(_imovelRepoMock.Object);
        _imovelRepoMock.Setup(repo => repo.CreateAsync(It.IsAny<Imovel>()))
            .Callback<Imovel>(imovel => imovel.Id = 101)
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Handle_ComandoValido_DeveChamarCreateAsyncERetornarSucessoDto()
    {
        var command = new CreateImovelCommand
        {
            Bloco = "Bloco B",
            Apartamento = "202",
            BoxGaragem = "B2"
        };

        var resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(101, resultado.Dados.Id);
        Assert.Equal(command.Bloco, resultado.Dados.Bloco);

        _imovelRepoMock.Verify(repo => repo.CreateAsync(
            It.Is<Imovel>(
                i => i.Bloco == command.Bloco && i.Apartamento == command.Apartamento
            )),
            Times.Once
        );
    }
}