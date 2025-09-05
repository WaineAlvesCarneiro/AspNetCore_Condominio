using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Queries.GetMoradorById;

public record GetMoradorByIdQuery(int Id) : IRequest<Result<MoradorDto>>;