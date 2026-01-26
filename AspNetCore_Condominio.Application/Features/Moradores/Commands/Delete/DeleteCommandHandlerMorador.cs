using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.Delete;

public class DeleteCommandHandlerMorador(IMoradorRepository repository)
    : IRequestHandler<DeleteCommandMorador, Result>
{
    public async Task<Result> Handle(DeleteCommandMorador request, CancellationToken cancellationToken)
    {
        var dado = await repository.GetByIdAsync(request.Id, request.UserEmpresaId);
        if (dado is null)
            return Result.Failure("Morador não encontrado.");

        await repository.DeleteAsync(dado);
        return Result.Success("Morador deletado com sucesso.");
    }
}