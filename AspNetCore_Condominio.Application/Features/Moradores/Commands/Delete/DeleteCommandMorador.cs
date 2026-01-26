using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.Delete;

public record DeleteCommandMorador(long Id, long UserEmpresaId) : IRequest<Result>;