using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Queries.GetAll;

public class GetAllQueryHandlerEmpresa(IEmpresaRepository repository)
    : IRequestHandler<GetAllQueryEmpresa, Result<IEnumerable<EmpresaDto>>>
{
    public async Task<Result<IEnumerable<EmpresaDto>>> Handle(GetAllQueryEmpresa request, CancellationToken cancellationToken)
    {
        var dados = await repository.GetAllAsync(
            empresaId: request.IdEmpresa, cancellationToken);

        var dtos = dados.Select(dado => dado.ToDto()).ToList();

        return Result<IEnumerable<EmpresaDto>>.Success(dtos);
    }
}
