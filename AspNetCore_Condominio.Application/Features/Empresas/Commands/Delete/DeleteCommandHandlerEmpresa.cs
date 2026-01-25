using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Delete;

public class DeleteCommandHandlerEmpresa(IEmpresaRepository repository, IImovelRepository imovelRepository)
    : IRequestHandler<DeleteCommandEmpresa, Result>
{
    public async Task<Result> Handle(DeleteCommandEmpresa request, CancellationToken cancellationToken)
    {
        var existsImovelVinculadoNaEmpresa = await imovelRepository.ExistsImovelVinculadoNaEmpresaAsync(request.Id);
        if (existsImovelVinculadoNaEmpresa)
            return Result.Failure("Não é possível excluir a empresa, pois tem Imóvel evinculado.");

        var dado = await repository.GetByIdAsync(request.Id);
        if (dado is null)
            return Result.Failure("Empresa não encontrada.");

        await repository.DeleteAsync(dado);
        return Result.Success("Empresa deletada com sucesso.");
    }
}