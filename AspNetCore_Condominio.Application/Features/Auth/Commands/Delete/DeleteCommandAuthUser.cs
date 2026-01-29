using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Delete;

public record DeleteCommandAuthUser(Guid Id) : IRequest<Result>;