using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllMoradores;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;
using DateOnly = System.DateOnly;

namespace AspNetCore_Condominio.Application.Tests.Features.Moradores.Querys.GetAllMoradores;

public class GetAllMoradoresQueryHandlerTests
{
    private readonly Mock<IMoradorRepository> _moradorRepoMock;
    private readonly GetAllMoradoresQueryHandler _handler;

    private readonly List<Morador> _moradoresFicticios = new List<Morador>
    {
        new Morador
        {
            Id = 1,
            Nome = "Alice Silva",
            Celular = "85991234567",
            Email = "alice@cond.com",
            DataEntrada = new DateTime(2024, 1, 10),
            DataInclusao = new DateTime(2024, 1, 10),
            IsProprietario = true,
            ImovelId = 5,
            Imovel = new Imovel {
                Id = 5,
                Bloco = "01",
                Apartamento = "101",
                BoxGaragem = "224"
            }
        },
        new Morador
        {
            Id = 2,
            Nome = "Bruno Lima",
            Celular = "31991234567",
            Email = "bruno@cond.com",
            DataEntrada = new DateTime(2024, 2, 1),
            DataInclusao = new DateTime(2024, 2, 1),
            DataSaida = new DateTime(2024, 9, 1),
            IsProprietario = false,
            ImovelId = 8
        }
    };

    public GetAllMoradoresQueryHandlerTests()
    {
        _moradorRepoMock = new Mock<IMoradorRepository>();
        _handler = new GetAllMoradoresQueryHandler(_moradorRepoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaDeMoradoresMapeadaParaDto()
    {
        var query = new GetAllMoradoresQuery();
        _moradorRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_moradoresFicticios);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        var dtos = resultado.Dados.ToList();

        Assert.Equal(_moradoresFicticios.Count, dtos.Count);

        var primeiroDto = dtos.First();

        Assert.Equal(1, primeiroDto.Id);
        Assert.Equal("Alice Silva", primeiroDto.Nome);
        Assert.Equal(DateOnly.FromDateTime(_moradoresFicticios[0].DataEntrada), primeiroDto.DataEntrada);
        Assert.True(primeiroDto.IsProprietario);
        Assert.NotNull(primeiroDto.ImovelDto);
        Assert.Equal("101", primeiroDto.ImovelDto.Apartamento);

        var segundoDto = dtos[1];

        Assert.NotNull(segundoDto.DataSaida);
        Assert.Equal(new DateOnly(2024, 9, 1), segundoDto.DataSaida);
        Assert.Null(segundoDto.ImovelDto);

        _moradorRepoMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoHaMoradores_DeveRetornarListaVazia()
    {
        var query = new GetAllMoradoresQuery();
        _moradorRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Morador>());
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Empty(resultado.Dados);

        _moradorRepoMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
}