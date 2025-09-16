using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllPagedMoradores;

public record GetAllPagedMoradoresQuery(
    int Page = 0,
    int LinesPerPage = 10,
    string OrderBy = "nome",
    string Direction = "ASC")
    : IRequest<Result<PagedResult<MoradorDto>>>;