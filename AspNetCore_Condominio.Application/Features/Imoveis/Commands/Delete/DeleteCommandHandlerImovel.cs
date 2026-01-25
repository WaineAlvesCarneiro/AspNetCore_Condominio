using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.Delete;

public class DeleteCommandHandlerImovel(IImovelRepository repository, IMoradorRepository moradorRepository)
    : IRequestHandler<DeleteCommandImovel, Result>
{
    public async Task<Result> Handle(DeleteCommandImovel request, CancellationToken cancellationToken)
    {
        var existsMoradorVinculadoNoImovel = await moradorRepository.ExistsMoradorVinculadoNoImovelAsync(request.Id);
        if (existsMoradorVinculadoNoImovel)
            return Result.Failure("Não é possível excluir o imóvel, pois existem moradores vinculados.");

        var dado = await repository.GetByIdAsync(request.Id);
        if (dado is null)
            return Result.Failure("Imóvel não encontrado.");

        await repository.DeleteAsync(dado);
        return Result.Success("Imóvel deletado com sucesso.");
    }
}