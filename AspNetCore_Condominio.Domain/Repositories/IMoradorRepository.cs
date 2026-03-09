using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Repositories;

public interface IMoradorRepository
{
    Task<IEnumerable<Morador>> GetAllAsync(long? empresaId = null, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Morador> items, int totalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        long? empresaId, string? nome, CancellationToken cancellationToken = default);
    Task<Morador?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task CreateAsync(Morador morador, CancellationToken cancellationToken = default);
    Task UpdateAsync(Morador morador, CancellationToken cancellationToken = default);
    Task DeleteAsync(Morador morador, CancellationToken cancellationToken = default);
    Task<bool> ExisteMoradorVinculadoNoImovelAsync(long imovelId, CancellationToken cancellationToken = default);
}