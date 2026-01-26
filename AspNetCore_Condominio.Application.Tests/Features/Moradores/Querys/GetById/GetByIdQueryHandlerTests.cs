using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetById;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;
using System;
using DateTime = System.DateTime;

namespace AspNetCore_Condominio.Application.Tests.Features.Moradores.Querys.GetById;

public class GetByIdQueryHandlerTests
{
    private readonly Mock<IMoradorRepository> _repoMock;
    private readonly GetByIdQueryHandlerMorador _handler;

    private const int ID_EXISTENTE = 42;
    private const int ID_NAO_EXISTENTE = 99;

    private readonly Morador _existente = new Morador
    {
        Id = ID_EXISTENTE,
        Nome = "Ricardo Query",
        Celular = "11990001111",
        Email = "ricardo@query.com",
        IsProprietario = true,
        DataEntrada = DateOnly.FromDateTime(new DateTime(2023, 5, 10, 8, 0, 0)),
        DataInclusao = new DateTime(2023, 5, 10, 8, 0, 0),
        DataSaida = DateOnly.FromDateTime(new DateTime(2024, 1, 15, 18, 30, 0)),
        DataAlteracao = new DateTime(2024, 1, 16, 9, 0, 0),
        ImovelId = 10,
        Imovel = new Imovel { Id = 10, Bloco = "Z", Apartamento = "999", BoxGaragem = "Z99", EmpresaId = 1 }
    };

    public GetByIdQueryHandlerTests()
    {
        _repoMock = new Mock<IMoradorRepository>();
        _handler = new GetByIdQueryHandlerMorador(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_MoradorExistente_DeveRetornarSucessoComMoradorDto()
    {
        var query = new GetByIdQueryMorador(ID_EXISTENTE);
        _repoMock.Setup(repo => repo.GetByIdAsync(ID_EXISTENTE)).ReturnsAsync(_existente);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        var dto = resultado.Dados;

        Assert.IsType<MoradorDto>(dto);
        Assert.Equal(ID_EXISTENTE, dto.Id);
        Assert.Equal(_existente.Nome, dto.Nome);


        Assert.Equal(DateOnly.FromDateTime(new DateTime(2023, 5, 10, 8, 0, 0)), dto.DataEntrada);
        Assert.Equal(DateOnly.FromDateTime(new DateTime(2024, 1, 15, 18, 30, 0)), dto.DataSaida);
        Assert.Equal(new DateTime(2024, 1, 16, 9, 0, 0), dto.DataAlteracao);

        Assert.NotNull(dto.ImovelDto);
        Assert.Equal(10, dto.ImovelDto.Id);
        Assert.Equal("Z", dto.ImovelDto.Bloco);

        _repoMock.Verify(repo => repo.GetByIdAsync(ID_EXISTENTE), Times.Once);
    }

    [Fact]
    public async Task Handle_MoradorInexistente_DeveRetornarFailure()
    {
        var query = new GetByIdQueryMorador(ID_NAO_EXISTENTE);
        _repoMock.Setup(repo => repo.GetByIdAsync(ID_NAO_EXISTENTE)).ReturnsAsync((Morador)null!);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Morador não encontrado.", resultado.Mensagem);
        Assert.Null(resultado.Dados);

        _repoMock.Verify(repo => repo.GetByIdAsync(ID_NAO_EXISTENTE), Times.Once);
    }
}
