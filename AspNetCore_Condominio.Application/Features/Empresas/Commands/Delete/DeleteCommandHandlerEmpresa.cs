using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Delete;

public class DeleteCommandHandlerEmpresa(IEmpresaRepository repository, IImovelRepository imovelRepository, IAuthUserRepository authUserRepository)
    : IRequestHandler<DeleteCommandEmpresa, Result>
{
    public async Task<Result> Handle(DeleteCommandEmpresa request, CancellationToken cancellationToken)
    {
        var existsImovelVinculadoNaEmpresa = await imovelRepository.ExisteImovelVinculadoNaEmpresaAsync(request.Id);
        if (existsImovelVinculadoNaEmpresa)
            return Result.Failure("Não é possível excluir a empresa, pois tem imóvel vinculado.");

        var existsUsuarioVinculadoNaEmpresa = await authUserRepository.ExisteUsuarioVinculadoNaEmpresaAsync(request.Id);
        if (existsUsuarioVinculadoNaEmpresa)
            return Result.Failure("Não é possível excluir o empresa, pois tem usuário vinculado.");

        var dado = await repository.GetByIdAsync(request.Id);
        if (dado is null)
            return Result.Failure("Empresa não encontrada.");

        await repository.DeleteAsync(dado);
        return Result.Success("Empresa deletada com sucesso.");
    }
}