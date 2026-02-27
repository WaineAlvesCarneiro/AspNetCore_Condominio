using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Repositories;

public interface IEmpresaRepository
{
    Task<IEnumerable<Empresa>> GetAllAsync(long? empresaId = null);
    Task<(IEnumerable<Empresa> Items, int TotalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        string? razaoSocial, string? cnpj);
    Task<Empresa?> GetByIdAsync(long id);
    Task CreateAsync(Empresa empresa);
    Task UpdateAsync(Empresa empresa);
    Task DeleteAsync(Empresa empresa);
    Task<bool> ExisteCnpjAsync(string cnpj, long id, CancellationToken cancellation);
}