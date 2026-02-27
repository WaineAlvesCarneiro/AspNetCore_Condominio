using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAll;

public record GetAllQueryImovel(
    long? EmpresaId = null)
        : IRequest<Result<IEnumerable<ImovelDto>>>
{
    public long? IdEmpresa => EmpresaId;
}