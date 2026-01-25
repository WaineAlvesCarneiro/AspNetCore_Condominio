using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAll;

public record GetAllQueryImovel() : IRequest<Result<IEnumerable<ImovelDto>>>;