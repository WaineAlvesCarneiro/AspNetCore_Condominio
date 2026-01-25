using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth;

public class AuthLoginQueryHandler(IAuthUserRepository authUserRepository) : IRequestHandler<AuthLoginQuery, AuthUser>
{
    private readonly IAuthUserRepository _authUserRepository = authUserRepository;

    public async Task<AuthUser> Handle(AuthLoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _authUserRepository.GetByUsernameAsync(request.Username);

        if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return null!;
        }

        return user;
    }
}