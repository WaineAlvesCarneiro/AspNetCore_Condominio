using AspNetCore_Condominio.Domain.Entities.Auth;

namespace AspNetCore_Condominio.Domain.Repositories.Auth;

public interface IAuthUserRepository
{
    Task<AuthUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<IEnumerable<AuthUser>> GetAllAsync(long? empresaId = null, CancellationToken cancellationToken = default);

    Task<(IEnumerable<AuthUser> Items, int TotalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        long? empresaId, string? userName, CancellationToken cancellationToken = default);

    Task<AuthUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task CreateAsync(AuthUser authUser, CancellationToken cancellationToken = default);

    Task UpdateAsync(AuthUser authUser, CancellationToken cancellationToken = default);

    Task DeleteAsync(AuthUser authUser, CancellationToken cancellationToken = default);

    Task<IEnumerable<AuthUser>> GetByEmpresaIdAsync(long empresaId, CancellationToken cancellationToken = default);

    Task<bool> ExisteUsuarioVinculadoNaEmpresaAsync(long id, CancellationToken cancellationToken = default);
}