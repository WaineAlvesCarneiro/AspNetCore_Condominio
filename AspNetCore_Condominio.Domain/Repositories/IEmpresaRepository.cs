using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Repositories;

public interface IEmpresaRepository
{
    Task<IEnumerable<Empresa>> GetAllAsync(long? empresaId = null, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Empresa> Items, int TotalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        string? razaoSocial, string? cnpj, CancellationToken cancellationToken = default);
    Task<Empresa?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task CreateAsync(Empresa empresa, CancellationToken cancellationToken = default);
    Task UpdateAsync(Empresa empresa, CancellationToken cancellationToken = default);
    Task DeleteAsync(Empresa empresa, CancellationToken cancellationToken = default);
    Task<bool> ExisteCnpjAsync(string cnpj, long id,CancellationToken cancellationToken = default);
}