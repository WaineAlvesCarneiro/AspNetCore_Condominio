using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Queries.GetById;

public record GetByIdQueryMorador(long Id, long UserEmpresaId) : IRequest<Result<MoradorDto>>;