using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Repositories;

public interface IImovelRepository
{
    Task<IEnumerable<Imovel>> GetAllAsync(long userEmpresaId);
    Task<(IEnumerable<Imovel> items, int totalCount)> GetAllPagedAsync(
        long userEmpresaId,
        int page = 1,
        int pageSize = 10,
        string? orderBy = "Id",
        string? direction = "ASC",
        string? searchTerm = null);
    Task<Imovel?> GetByIdAsync(long id, long userEmpresaId);
    Task CreateAsync(Imovel imovel);
    Task UpdateAsync(Imovel imovel);
    Task DeleteAsync(Imovel imovel);
    Task<bool> ExisteImovelVinculadoNaEmpresaAsync(long empresaId);
}