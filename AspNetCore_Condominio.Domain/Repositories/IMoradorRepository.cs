using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Repositories;

public interface IMoradorRepository
{
    Task<IEnumerable<Morador>> GetAllAsync(long? empresaId = null);
    Task<(IEnumerable<Morador> items, int totalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        long? empresaId, string? nome);
    Task<Morador?> GetByIdAsync(long id);
    Task CreateAsync(Morador morador);
    Task UpdateAsync(Morador morador);
    Task DeleteAsync(Morador morador);
    Task<bool> ExisteMoradorVinculadoNoImovelAsync(long imovelId);
}