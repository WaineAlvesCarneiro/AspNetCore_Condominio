using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Events;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.CreateMorador;

public class CreateMoradorCommandHandler(IMoradorRepository moradorRepository, IImovelRepository imovelRepository, IMediator mediator)
    : IRequestHandler<CreateMoradorCommand, Result<MoradorDto>>
{
    private readonly IMoradorRepository _moradorRepository = moradorRepository;
    private readonly IImovelRepository _imovelRepository = imovelRepository;

    public async Task<Result<MoradorDto>> Handle(CreateMoradorCommand request, CancellationToken cancellationToken)
    {
        var imovelExist = await _imovelRepository.GetByIdAsync(request.ImovelId);
        if (imovelExist == null)
        {
            return Result<MoradorDto>.Failure("O imóvel informado não existe.");
        }

        var morador = new Morador
        {
            Nome = request.Nome,
            Celular = request.Celular,
            Email = request.Email,
            IsProprietario = request.IsProprietario,
            DataEntrada = request.DataEntrada.ToDateTime(TimeOnly.MinValue),
            DataSaida = null,
            DataInclusao = request.DataInclusao.ToDateTime(TimeOnly.MinValue),
            DataAlteracao = null,
            ImovelId = request.ImovelId
        };

        var horaAgora = DateTime.Now.TimeOfDay;

        var dataEntradaHoje = morador.DataEntrada.Date;
        var dataEntradaComHora = dataEntradaHoje + horaAgora;
        morador.DataEntrada = dataEntradaComHora;

        var dataInclusaoHoje = morador.DataInclusao.Date;
        var dataInclusaoComHora = dataInclusaoHoje + horaAgora;
        morador.DataInclusao = dataInclusaoComHora;

        await _moradorRepository.CreateAsync(morador);

        // Publica o evento!
        await mediator.Publish(new MoradorCriadoEvent(morador, true), cancellationToken);

        var dto = new MoradorDto
        {
            Id = morador.Id,
            Nome = morador.Nome,
            Celular = morador.Celular,
            Email = morador.Email,
            IsProprietario = morador.IsProprietario,
            DataEntrada = DateOnly.FromDateTime(morador.DataEntrada),
            DataSaida = morador.DataSaida.HasValue ? DateOnly.FromDateTime(morador.DataSaida.Value) : null,
            DataInclusao = DateOnly.FromDateTime(morador.DataInclusao),
            DataAlteracao = morador.DataAlteracao.HasValue ? DateOnly.FromDateTime(morador.DataAlteracao.Value) : null,
            ImovelId = morador.ImovelId,
            ImovelDto = morador.Imovel != null ? new ImovelDto
            {
                Id = morador.Imovel.Id,
                Bloco = morador.Imovel.Bloco,
                Apartamento = morador.Imovel.Apartamento,
                BoxGaragem = morador.Imovel.BoxGaragem
            } : null
        };

        return Result<MoradorDto>.Success(dto, "Morador criado com sucesso.");
    }
}
