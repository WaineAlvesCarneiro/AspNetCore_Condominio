using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.DeleteImovel;

public record DeleteImovelCommand(int Id) : IRequest<Result>;