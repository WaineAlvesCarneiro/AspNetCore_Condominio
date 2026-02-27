using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Delete;

public record DeleteCommandEmpresa(long Id) : IRequest<Result>;