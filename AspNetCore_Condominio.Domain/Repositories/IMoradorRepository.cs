using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Repositories;

public interface IMoradorRepository
{
    Task<IEnumerable<Morador>> GetAllAsync();
    Task<(IEnumerable<Morador> Items, int TotalCount)> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction);
    Task<Morador?> GetByIdAsync(int id);
    Task CreateAsync(Morador morador);
    Task UpdateAsync(Morador morador);
    Task DeleteAsync(Morador morador);
    Task<bool> ExistsByImovelIdAsync(int imovelId);
}