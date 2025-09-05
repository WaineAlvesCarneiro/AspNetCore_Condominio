using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAllPagedImoveis;

public record GetAllPagedImoveisQuery(
    int Page = 0,
    int LinesPerPage = 10,
    string OrderBy = "id",
    string Direction = "ASC")
    : IRequest<Result<PagedResult<ImovelDto>>>;