using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Delete;

public class DeleteCommandHandlerAuthUser(IAuthUserRepository repository)
    : IRequestHandler<DeleteCommandAuthUser, Result>
{
    public async Task<Result> Handle(DeleteCommandAuthUser request, CancellationToken cancellationToken)
    {
        var dado = await repository.GetByIdAsync(request.Id);
        if (dado is null)
            return Result.Failure("Usuário não encontrado.");

        await repository.DeleteAsync(dado);
        return Result.Success("Usuário deletado com sucesso.");
    }
}