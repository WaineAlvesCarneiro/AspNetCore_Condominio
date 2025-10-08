using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetMoradorById;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;
using DateOnly = System.DateOnly;

namespace AspNetCore_Condominio.Application.Tests.Features.Moradores.Querys.GetMoradorById;

public class GetMoradorByIdQueryHandlerTests
{
    private readonly Mock<IMoradorRepository> _moradorRepoMock;
    private readonly GetMoradorByIdQueryHandler _handler;

    private const int MORADOR_ID_EXISTENTE = 42;
    private const int MORADOR_ID_INEXISTENTE = 99;

    private readonly Morador _moradorExistente = new Morador
    {
        Id = MORADOR_ID_EXISTENTE,
        Nome = "Ricardo Query",
        Celular = "11990001111",
        Email = "ricardo@query.com",
        IsProprietario = true,
        DataEntrada = new DateTime(2023, 5, 10, 8, 0, 0),
        DataInclusao = new DateTime(2023, 5, 10, 8, 0, 0),
        DataSaida = new DateTime(2024, 1, 15, 18, 30, 0),
        DataAlteracao = new DateTime(2024, 1, 16, 9, 0, 0),
        ImovelId = 10,
        Imovel = new Imovel { Id = 10, Bloco = "Z", Apartamento = "999", BoxGaragem = "Z99" }
    };

    public GetMoradorByIdQueryHandlerTests()
    {
        _moradorRepoMock = new Mock<IMoradorRepository>();
        _handler = new GetMoradorByIdQueryHandler(_moradorRepoMock.Object);
    }

    [Fact]
    public async Task Handle_MoradorExistente_DeveRetornarSucessoComMoradorDto()
    {
        var query = new GetMoradorByIdQuery(MORADOR_ID_EXISTENTE);
        _moradorRepoMock.Setup(repo => repo.GetByIdAsync(MORADOR_ID_EXISTENTE)).ReturnsAsync(_moradorExistente);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        var dto = resultado.Dados;

        Assert.IsType<MoradorDto>(dto);
        Assert.Equal(MORADOR_ID_EXISTENTE, dto.Id);
        Assert.Equal(_moradorExistente.Nome, dto.Nome);


        Assert.Equal(new DateOnly(2023, 5, 10), dto.DataEntrada);
        Assert.Equal(new DateOnly(2024, 1, 15), dto.DataSaida);
        Assert.Equal(new DateOnly(2024, 1, 16), dto.DataAlteracao);

        Assert.NotNull(dto.ImovelDto);
        Assert.Equal(10, dto.ImovelDto.Id);
        Assert.Equal("Z", dto.ImovelDto.Bloco);

        _moradorRepoMock.Verify(repo => repo.GetByIdAsync(MORADOR_ID_EXISTENTE), Times.Once);
    }

    [Fact]
    public async Task Handle_MoradorInexistente_DeveRetornarFailure()
    {
        var query = new GetMoradorByIdQuery(MORADOR_ID_INEXISTENTE);
        _moradorRepoMock.Setup(repo => repo.GetByIdAsync(MORADOR_ID_INEXISTENTE)).ReturnsAsync((Morador)null!);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Morador não encontrado.", resultado.Mensagem);
        Assert.Null(resultado.Dados);

        _moradorRepoMock.Verify(repo => repo.GetByIdAsync(MORADOR_ID_INEXISTENTE), Times.Once);
    }
}
