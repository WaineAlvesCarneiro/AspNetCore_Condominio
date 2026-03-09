using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAll;

public record GetAllQueryMorador(
    long? EmpresaId = null)
        : IRequest<Result<IEnumerable<MoradorDto>>>
{
    public long? IdEmpresa => EmpresaId;
}