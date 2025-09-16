using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.DeleteImovel;

public record DeleteImovelCommand(int Id) : IRequest<Result>;