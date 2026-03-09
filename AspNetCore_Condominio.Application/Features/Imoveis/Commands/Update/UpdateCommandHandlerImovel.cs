using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.Update;

public class UpdateCommandHandlerImovel(
    IImovelRepository repository)
        : IRequestHandler<UpdateCommandImovel, Result<ImovelDto>>
{
    public async Task<Result<ImovelDto>> Handle(UpdateCommandImovel request, CancellationToken cancellationToken)
    {
        var dadoToUpdate = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (dadoToUpdate == null)
            return Result<ImovelDto>.Failure("Imóvel não encontrado.");

        dadoToUpdate.UpdateFromCommand(request);
        await repository.UpdateAsync(dadoToUpdate, cancellationToken);

        return Result<ImovelDto>.Success(dadoToUpdate.ToDto(), "Imóvel atualizado com sucesso.");
    }
}