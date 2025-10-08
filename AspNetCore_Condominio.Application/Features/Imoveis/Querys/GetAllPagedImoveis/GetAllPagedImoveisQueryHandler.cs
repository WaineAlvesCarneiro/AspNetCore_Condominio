using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAllPagedImoveis;

public class GetAllPagedImoveisQueryHandler(IImovelRepository imovelRepository)
    : IRequestHandler<GetAllPagedImoveisQuery, Result<PagedResult<ImovelDto>>>
{
    private readonly IImovelRepository _imovelRepository = imovelRepository;

    public async Task<Result<PagedResult<ImovelDto>>> Handle(GetAllPagedImoveisQuery request, CancellationToken cancellationToken)
    {
        (IEnumerable<Imovel> items, int totalCount) = await _imovelRepository.GetAllPagedAsync(
            request.Page,
            request.LinesPerPage,
            request.OrderBy,
            request.Direction
        );

        IEnumerable<ImovelDto> dtos = items.Select(i => new ImovelDto
        {
            Id = i.Id,
            Bloco = i.Bloco,
            Apartamento = i.Apartamento,
            BoxGaragem = i.BoxGaragem
        });

        PagedResult<ImovelDto> pagedResult = new PagedResult<ImovelDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = request.Page,
            LinesPerPage = request.LinesPerPage
        };

        return Result<PagedResult<ImovelDto>>.Success(pagedResult);
    }
}
