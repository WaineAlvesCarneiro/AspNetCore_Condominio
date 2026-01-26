using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.Delete;

public record DeleteCommandImovel(long Id, long UserEmpresaId) : IRequest<Result>;