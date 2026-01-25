using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.Application.Features.Empresas.Queries.GetAllPaged;

public record GetAllPagedQueryEmpresa(
    [FromQuery(Name = "page")] int Page = 1,
    [FromQuery(Name = "pageSize")] int PageSize = 10,
    [FromQuery(Name = "sortBy")] string? SortBy = "Id",
    [FromQuery(Name = "sortDescending")] bool SortDescending = false,
    [FromQuery(Name = "searchTerm")] string? SearchTerm = null) : IRequest<Result<PagedResult<EmpresaDto>>>
{
    public int ActualPage => Page < 1 ? 1 : Page;
    public int ActualPageSize => PageSize < 1 ? 10 : PageSize;
    public string ActualSortBy => !string.IsNullOrWhiteSpace(SortBy) ? SortBy : "Id";
    public string ActualDirection => SortDescending ? "DESC" : "ASC";
}