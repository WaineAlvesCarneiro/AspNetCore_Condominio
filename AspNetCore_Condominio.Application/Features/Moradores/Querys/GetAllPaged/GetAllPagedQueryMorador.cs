using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllPaged;

public record GetAllPagedQueryMorador(
    long UserEmpresaId,
    int Page = 1,
    int PageSize = 10,
    string? SortBy = "Id",
    string SortDescending = "ASC",
    string? SearchTerm = null)
    : IRequest<Result<PagedResult<MoradorDto>>>
{
    public int ActualPage => Page < 1 ? 1 : Page;
    public int ActualPageSize => PageSize < 1 ? 10 : PageSize;
    public string ActualSortBy => !string.IsNullOrWhiteSpace(SortBy) ? SortBy : "Id";
    public string ActualDirection => SortDescending;
}