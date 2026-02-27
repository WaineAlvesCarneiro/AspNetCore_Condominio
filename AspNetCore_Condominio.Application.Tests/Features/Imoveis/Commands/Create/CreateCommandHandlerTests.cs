using AspNetCore_Condominio.Application.Features.Imoveis.Commands.Create;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.Create;

public class CreateCommandHandlerTests
{
    private readonly Mock<IImovelRepository> _repoMock;
    private readonly CreateCommandHandlerImovel _handler;

    public CreateCommandHandlerTests()
    {
        _repoMock = new Mock<IImovelRepository>();
        _handler = new CreateCommandHandlerImovel(_repoMock.Object);
        _repoMock.Setup(repo => repo.CreateAsync(It.IsAny<Imovel>()))
            .Callback<Imovel>(imovel => imovel.Id = 101)
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Handle_ComandoValido_DeveChamarCreateAsyncERetornarSucessoDto()
    {
        // Arrange
        long idGerado = 101;
        CreateCommandImovel command = new()
        {
            Bloco = "Bloco B",
            Apartamento = "202",
            BoxGaragem = "B2"
        };

        // Act
        Domain.Common.Result<DTOs.ImovelDto> resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(idGerado, resultado.Dados.Id);
        Assert.Equal(command.Bloco, resultado.Dados.Bloco);

        _repoMock.Verify(repo => repo.CreateAsync(
            It.Is<Imovel>(
                i => i.Bloco == command.Bloco && i.Apartamento == command.Apartamento
            )),
            Times.Once
        );
    }
}