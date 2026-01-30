using AspNetCore_Condominio.Domain.Entities.Auth;

namespace AspNetCore_Condominio.Domain.Repositories.Auth;
public interface IAuthUserRepository
{
    Task<AuthUser> GetByUsernameAsync(string username);

    Task<IEnumerable<AuthUser>> GetAllAsync();
    Task<(IEnumerable<AuthUser> Items, int TotalCount)> GetAllPagedAsync(
        int page = 1,
        int pageSize = 10,
        string? orderBy = "Id",
        string? direction = "ASC",
        string? searchTerm = null);
    Task<AuthUser?> GetByIdAsync(Guid id);
    Task CreateAsync(AuthUser authUser);
    Task UpdateAsync(AuthUser authUser);
    Task DeleteAsync(AuthUser authUser);
    Task<bool> ExisteUsuarioVinculadoNaEmpresaAsync(long id);
}
