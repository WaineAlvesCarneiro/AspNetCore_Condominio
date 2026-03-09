using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Queries.GetById;

public class GetByIdQueryHandlerEmpresa(IEmpresaRepository repository)
    : IRequestHandler<GetByIdQueryEmpresa, Result<EmpresaDto>>
{
    public async Task<Result<EmpresaDto>> Handle(GetByIdQueryEmpresa request, CancellationToken cancellationToken)
    {
        var dado = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (dado is null)
            return Result<EmpresaDto>.Failure("Empresa não encontrada.");

        return Result<EmpresaDto>.Success(dado.ToDto());
    }
}