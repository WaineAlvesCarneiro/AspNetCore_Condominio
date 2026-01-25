using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using AspNetCore_Condominio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore_Condominio.Infrastructure.Repositories.Auth;

public class AuthUserRepository(ApplicationDbContext context) : IAuthUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<AuthUser> GetByUsernameAsync(string username)
    {
        var users = await _context.AuthUsers.ToListAsync();

        return users.FirstOrDefault(u => string.Equals(u.UserName, username, StringComparison.OrdinalIgnoreCase));
    }
}

