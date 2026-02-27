using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Repositories;

public interface IImovelRepository
{
    Task<IEnumerable<Imovel>> GetAllAsync(long? empresaId = null);
    Task<(IEnumerable<Imovel> items, int totalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        long? empresaId, string? bloco, string? apartamento);
    Task<Imovel?> GetByIdAsync(long id);
    Task CreateAsync(Imovel imovel);
    Task UpdateAsync(Imovel imovel);
    Task DeleteAsync(Imovel imovel);
    Task<bool> ExisteImovelVinculadoNaEmpresaAsync(long empresaId);
}