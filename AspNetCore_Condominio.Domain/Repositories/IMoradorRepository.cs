using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Repositories;

public interface IMoradorRepository
{
    Task<IEnumerable<Morador>> GetAllAsync(long userEmpresaId);
    Task<(IEnumerable<Morador> items, int totalCount)> GetAllPagedAsync(
        long userEmpresaId,
        int page = 1,
        int pageSize = 10,
        string? orderBy = "Id",
        string? direction = "ASC",
        string? searchTerm = null);
    Task<Morador?> GetByIdAsync(long id, long userEmpresaId);
    Task CreateAsync(Morador morador);
    Task UpdateAsync(Morador morador);
    Task DeleteAsync(Morador morador);
    Task<bool> ExisteMoradorVinculadoNoImovelAsync(long imovelId);
}