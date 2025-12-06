using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAllImoveis;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Querys.GetAllImoveis;

public class GetAllImoveisQueryHandlerTests
{
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly GetAllImoveisQueryHandler _handler;

    private readonly List<Imovel> _imoveisFicticios =
    [
        new Imovel { Id = 1, Bloco = "A", Apartamento = "101", BoxGaragem = "A1" },
        new Imovel { Id = 2, Bloco = "B", Apartamento = "202", BoxGaragem = "B2" }
    ];

    public GetAllImoveisQueryHandlerTests()
    {
        // SUT
        // System Under Test
        // Objeto sendo testado
        // No caso, o handler do comando de obtenção de todos os imóveis
        // Estamos injetando o mock do repositório de imóveis para isolar o teste e controlar o comportamento do repositório
        _imovelRepoMock = new Mock<IImovelRepository>();
        _handler = new GetAllImoveisQueryHandler(_imovelRepoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaDeImoveisMapeadaParaDto()
    {
        // Arrange
        int idprimeiroDto = 1;
        string blocoPrimeiroDto = "A";
        GetAllImoveisQuery query = new();

        /*
        _imovelRepoMock: Este é o seu objeto mock. Que é a instância de Mock<IImovelRepository> criada anteriormente.
            Ele simula o comportamento do repositório de imóveis (ImovelRepo).
        .Setup(repo => repo.GetAllAsync()):
        .Setup(...): É o método do Moq que diz: "Quando este método for chamado no objeto mock, eu quero que você faça algo específico."
        repo => repo.GetAllAsync(): É uma expressão lambda que especifica o método exato que você está simulando.
            Neste caso, é o método assíncrono GetAllAsync() que retorna um Task<IEnumerable<ImovelDto>>.
        .ReturnsAsync(_imoveisFicticios):
        .ReturnsAsync(...): É um método auxiliar do Moq, usado especificamente para métodos assíncronos (que retornam Task ou Task<T>).
            Ele diz qual valor deve ser retornado quando o método simulado (GetAllAsync()) for chamado. O Moq se encarrega de empacotar esse valor em uma Task concluída.
        _imoveisFicticios: É uma lista ou coleção de objetos de imóvel criada previamente em seu teste. 
            Esse é o dado fictício (o mock data) que você quer que o GetAllAsync() "retorne" durante a execução do teste.
        */
        _imovelRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_imoveisFicticios);

        // Act
        Domain.Common.Result<IEnumerable<ImovelDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        List<ImovelDto> dtos = resultado.Dados.ToList();

        Assert.Equal(_imoveisFicticios.Count, dtos.Count);

        ImovelDto primeiroDto = dtos.First();

        Assert.IsType<ImovelDto>(primeiroDto);
        Assert.Equal(idprimeiroDto, primeiroDto.Id);
        Assert.Equal(blocoPrimeiroDto, primeiroDto.Bloco);

        _imovelRepoMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoHaImoveis_DeveRetornarListaVazia()
    {
        // Arrange
        GetAllImoveisQuery query = new();
        _imovelRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Imovel>());

        // Act
        Domain.Common.Result<IEnumerable<ImovelDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Empty(resultado.Dados);

        _imovelRepoMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
}
