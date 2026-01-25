using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAllPaged;

public record GetAllPagedQueryHandlerImovel(IImovelRepository repository)
    : IRequestHandler<GetAllPagedQueryImovel, Result<PagedResult<ImovelDto>>>
{
    private readonly IImovelRepository _repository = repository;

    public async Task<Result<PagedResult<ImovelDto>>> Handle(
        GetAllPagedQueryImovel request,
        CancellationToken cancellationToken)
    {
        (IEnumerable<Imovel> items, int totalCount) = await _repository.GetAllPagedAsync(
            page: request.ActualPage,
            pageSize: request.ActualPageSize,
            orderBy: request.ActualSortBy,
            direction: request.ActualDirection,
            searchTerm: request.SearchTerm
        );

        IEnumerable<ImovelDto> dtos = items.Select(dado => new ImovelDto
        {
            Id = dado.Id,
            Bloco = dado.Bloco,
            Apartamento = dado.Apartamento,
            BoxGaragem = dado.BoxGaragem,
            EmpresaId = dado.EmpresaId
        });

        PagedResult<ImovelDto> pagedResult = new PagedResult<ImovelDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = request.ActualPage,
            LinesPerPage = request.ActualPageSize
        };

        return Result<PagedResult<ImovelDto>>.Success(pagedResult);
    }
}