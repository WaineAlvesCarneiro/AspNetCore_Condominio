using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Events;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.UpdateMorador;

public class UpdateMoradorCommandHandler(IMoradorRepository moradorRepository, IImovelRepository imovelRepository, IMediator mediator)
    : IRequestHandler<UpdateMoradorCommand, Result<MoradorDto>>
{
    private readonly IMoradorRepository _moradorRepository = moradorRepository;
    private readonly IImovelRepository _imovelRepository = imovelRepository;

    public async Task<Result<MoradorDto>> Handle(UpdateMoradorCommand request, CancellationToken cancellationToken)
    {
        var moradorToUpdate = await _moradorRepository.GetByIdAsync(request.Id);
        if (moradorToUpdate == null)
        {
            return Result<MoradorDto>.Failure("Morador não encontrado.");
        }

        var imovelExist = await _imovelRepository.GetByIdAsync(request.ImovelId);
        if (imovelExist == null)
        {
            return Result<MoradorDto>.Failure("O imóvel informado não existe.");
        }

        var horaAgora = DateTime.Now.TimeOfDay;

        moradorToUpdate.Nome = request.Nome;
        moradorToUpdate.Celular = request.Celular;
        moradorToUpdate.Email = request.Email;
        moradorToUpdate.IsProprietario = request.IsProprietario;

        var dataValidaEntrada = moradorToUpdate.DataEntrada;

        if (request.DataEntrada != DateOnly.FromDateTime(dataValidaEntrada))
        {
            moradorToUpdate.DataEntrada = request.DataEntrada.ToDateTime(TimeOnly.MinValue);

            var dataEntradaHoje = moradorToUpdate.DataEntrada.Date;
            var dataEntradaComHora = dataEntradaHoje + horaAgora;
            moradorToUpdate.DataEntrada = dataEntradaComHora;
        }

        var dataValidaSaidaToUpdate = moradorToUpdate.DataSaida;
        var dataValidaSaidaRequest = request.DataSaida?.ToDateTime(TimeOnly.MinValue);

        if (dataValidaSaidaRequest != moradorToUpdate.DataSaida?.Date)
        {
            moradorToUpdate.DataSaida = request.DataSaida?.ToDateTime(TimeOnly.MinValue);

            var dataSaidaHoje = moradorToUpdate.DataSaida?.Date;
            var dataSaidaComHora = dataSaidaHoje + horaAgora;
            moradorToUpdate.DataSaida = dataSaidaComHora;
        }

        moradorToUpdate.ImovelId = request.ImovelId;
        moradorToUpdate.DataAlteracao = request.DataAlteracao?.ToDateTime(TimeOnly.MinValue);

        moradorToUpdate.DataInclusao = moradorToUpdate.DataInclusao;

        var dataAlteracaoHoje = moradorToUpdate.DataAlteracao?.Date;
        var dataAlteracaoComHora = dataAlteracaoHoje + horaAgora;
        moradorToUpdate.DataAlteracao = dataAlteracaoComHora;

        await _moradorRepository.UpdateAsync(moradorToUpdate);

        // Publica o evento!
        await mediator.Publish(new MoradorCriadoEvent(moradorToUpdate, false), cancellationToken);

        var dto = new MoradorDto
        {
            Id = moradorToUpdate.Id,
            Nome = moradorToUpdate.Nome,
            Celular = moradorToUpdate.Celular,
            Email = moradorToUpdate.Email,
            IsProprietario = moradorToUpdate.IsProprietario,
            DataEntrada = DateOnly.FromDateTime(moradorToUpdate.DataEntrada),
            DataSaida = moradorToUpdate.DataSaida.HasValue ? DateOnly.FromDateTime(moradorToUpdate.DataSaida.Value) : null,
            DataInclusao = DateOnly.FromDateTime(moradorToUpdate.DataInclusao),
            DataAlteracao = moradorToUpdate.DataAlteracao.HasValue ? DateOnly.FromDateTime(moradorToUpdate.DataAlteracao.Value) : null,
            ImovelId = moradorToUpdate.ImovelId,
            ImovelDto = moradorToUpdate.Imovel != null ? new ImovelDto
            {
                Id = moradorToUpdate.Imovel.Id,
                Bloco = moradorToUpdate.Imovel.Bloco,
                Apartamento = moradorToUpdate.Imovel.Apartamento,
                BoxGaragem = moradorToUpdate.Imovel.BoxGaragem
            } : null
        };

        return Result<MoradorDto>.Success(dto, "Morador atualizado com sucesso.");
    }
}