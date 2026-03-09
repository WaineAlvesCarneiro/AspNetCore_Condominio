using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllPaged;

public class GetAllPagedQueryHandlerMorador(IMoradorRepository repository)
    : IRequestHandler<GetAllPagedQueryMorador, Result<PagedResult<MoradorDto>>>
{
    private readonly IMoradorRepository _repository = repository;

    public async Task<Result<PagedResult<MoradorDto>>> Handle(
        GetAllPagedQueryMorador request,
        CancellationToken cancellationToken)
    {
        (IEnumerable<Morador> items, int totalCount) = await _repository.GetAllPagedAsync(
            page: request.ActualPage,
            pageSize: request.ActualPageSize,
            orderBy: request.ActualSortBy,
            direction: request.ActualDirection,
            empresaId: request.EmpresaId,
            nome: request.Nome,
            cancellationToken: cancellationToken);

        var dtos = items.Select(dado => dado.ToDto()).ToList();

        return Result<PagedResult<MoradorDto>>.Success(new PagedResult<MoradorDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = request.ActualPage,
            LinesPerPage = request.ActualPageSize
        });
    }
}