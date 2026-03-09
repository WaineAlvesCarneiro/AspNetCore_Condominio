using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetById;

public record GetByIdQueryImovel(long Id) : IRequest<Result<ImovelDto>>;