using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Repositories;

public interface IMoradorRepository
{
    Task<IEnumerable<Morador>> GetAllAsync();
    Task<(IEnumerable<Morador> items, int totalCount)> GetAllPagedAsync(
        int page = 1,
        int pageSize = 10,
        string? orderBy = "Id",
        string? direction = "ASC",
        string? searchTerm = null);
    Task<Morador?> GetByIdAsync(long id);
    Task CreateAsync(Morador morador);
    Task UpdateAsync(Morador morador);
    Task DeleteAsync(Morador morador);
    Task<bool> ExistsMoradorVinculadoNoImovelAsync(long imovelId);
}