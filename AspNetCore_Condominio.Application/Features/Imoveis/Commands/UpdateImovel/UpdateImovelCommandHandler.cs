using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.UpdateImovel;

public class UpdateImovelCommandHandler(IImovelRepository imovelRepository)
    : IRequestHandler<UpdateImovelCommand, Result<ImovelDto>>
{
    private readonly IImovelRepository _imovelRepository = imovelRepository;

    public async Task<Result<ImovelDto>> Handle(UpdateImovelCommand request, CancellationToken cancellationToken)
    {
        var imovelToUpdate = await _imovelRepository.GetByIdAsync(request.Id);
        if (imovelToUpdate == null)
        {
            return Result<ImovelDto>.Failure("Imóvel não encontrado.");
        }

        imovelToUpdate.Bloco = request.Bloco;
        imovelToUpdate.Apartamento = request.Apartamento;
        imovelToUpdate.BoxGaragem = request.BoxGaragem;

        await _imovelRepository.UpdateAsync(imovelToUpdate);

        var dto = new ImovelDto
        {
            Id = imovelToUpdate.Id,
            Bloco = imovelToUpdate.Bloco,
            Apartamento = imovelToUpdate.Apartamento,
            BoxGaragem = imovelToUpdate.BoxGaragem
        };

        return Result<ImovelDto>.Success(dto, "Imóvel criado com sucesso.");
    }
}