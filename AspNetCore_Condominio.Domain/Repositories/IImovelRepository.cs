using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Repositories;

public interface IImovelRepository
{
    Task<IEnumerable<Imovel>> GetAllAsync();
    Task<(IEnumerable<Imovel> Items, int TotalCount)> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction);
    Task<Imovel?> GetByIdAsync(int id);
    Task CreateAsync(Imovel imovel);
    Task UpdateAsync(Imovel imovel);
    Task DeleteAsync(Imovel imovel);
}