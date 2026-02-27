using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Queries.GetAll;

public record GetAllQueryAuthUser(
    long? EmpresaId = null)
        : IRequest<Result<IEnumerable<AuthUserDto>>>
{
    public long? IdEmpresa => EmpresaId;
}