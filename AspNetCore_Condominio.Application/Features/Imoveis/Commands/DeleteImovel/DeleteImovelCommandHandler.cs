using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.DeleteImovel;

public class DeleteImovelCommandHandler(IImovelRepository imovelRepository, IMoradorRepository moradorRepository)
    : IRequestHandler<DeleteImovelCommand, Result>
{
    public async Task<Result> Handle(DeleteImovelCommand request, CancellationToken cancellationToken)
    {
        var possuiMorador = await moradorRepository.ExistsByImovelIdAsync(request.Id);
        if (possuiMorador)
            return Result.Failure("Não é possível excluir o imóvel, pois existem moradores vinculados.");

        var imovel = await imovelRepository.GetByIdAsync(request.Id);
        if (imovel is null)
            return Result.Failure("Imóvel não encontrado.");

        await imovelRepository.DeleteAsync(imovel);
        return Result.Success("Imóvel deletado com sucesso.");
    }
}