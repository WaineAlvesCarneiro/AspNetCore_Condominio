using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Queries.GetAllPaged;

public record GetAllPagedQueryEmpresa(
    int Page = 1,
    int PageSize = 10,
    string? SortBy = "Id",
    string SortDescending = "ASC",
    string? SearchTerm = null)
    : IRequest<Result<PagedResult<EmpresaDto>>>
{
    public int ActualPage => Page < 1 ? 1 : Page;
    public int ActualPageSize => PageSize < 1 ? 10 : PageSize;
    public string ActualSortBy => !string.IsNullOrWhiteSpace(SortBy) ? SortBy : "Id";
    public string ActualDirection => SortDescending;
}