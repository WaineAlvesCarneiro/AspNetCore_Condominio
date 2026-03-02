using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Repositories;

public interface IImovelRepository
{
    Task<IEnumerable<Imovel>> GetAllAsync(long? empresaId = null, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Imovel> items, int totalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        long? empresaId, string? bloco, string? apartamento, CancellationToken cancellationToken = default);
    Task<Imovel?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task CreateAsync(Imovel imovel, CancellationToken cancellationToken = default);
    Task UpdateAsync(Imovel imovel, CancellationToken cancellationToken = default);
    Task DeleteAsync(Imovel imovel, CancellationToken cancellationToken = default);
    Task<bool> ExisteImovelVinculadoNaEmpresaAsync(long empresaId, CancellationToken cancellationToken = default);
}