using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Queries.GetAll;

public record GetAllQueryAuthUser() : IRequest<Result<IEnumerable<AuthUserDto>>>;