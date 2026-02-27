using AspNetCore_Condominio.Domain.Entities.Auth;

namespace AspNetCore_Condominio.Domain.Repositories.Auth;
public interface IAuthUserRepository
{
    Task<AuthUser> GetByUsernameAsync(string username);
    Task<IEnumerable<AuthUser>> GetAllAsync(long? empresaId = null);
    Task<(IEnumerable<AuthUser> Items, int TotalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        long? empresaId, string? userName);
    Task<AuthUser?> GetByIdAsync(Guid id);
    Task CreateAsync(AuthUser authUser);
    Task UpdateAsync(AuthUser authUser);
    Task DeleteAsync(AuthUser authUser);
    Task<IEnumerable<AuthUser>> GetByEmpresaIdAsync(long empresaId);
    Task<bool> ExisteUsuarioVinculadoNaEmpresaAsync(long id);
}
