using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Queries.GetAll;

public record GetAllQueryEmpresa(
    long? EmpresaId = null)
        : IRequest<Result<IEnumerable<EmpresaDto>>>
{
    public long? IdEmpresa => EmpresaId;
}