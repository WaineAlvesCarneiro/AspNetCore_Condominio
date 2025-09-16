using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.DeleteMorador;

public class DeleteMoradorCommandHandler(IMoradorRepository moradorRepository)
    : IRequestHandler<DeleteMoradorCommand, Result>
{
    public async Task<Result> Handle(DeleteMoradorCommand request, CancellationToken cancellationToken)
    {
        var morador = await moradorRepository.GetByIdAsync(request.Id);
        if (morador is null)
            return Result.Failure("Morador não encontrado.");

        await moradorRepository.DeleteAsync(morador);
        return Result.Success("Morador deletado com sucesso.");
    }
}