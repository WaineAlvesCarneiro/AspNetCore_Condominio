using AspNetCore_Condominio.Domain.Entities.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth;

public class AuthLoginQuery : IRequest<AuthUser>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}