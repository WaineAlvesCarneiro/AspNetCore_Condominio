using AspNetCore_Condominio.Domain.Entities.Auth;

namespace AspNetCore_Condominio.Domain.Repositories.Auth;
public interface IAuthUserRepository
{
    Task<AuthUser> GetByUsernameAsync(string username);
}
