using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Domain.Repositories;

public interface IEmpresaRepository
{
    Task<IEnumerable<Empresa>> GetAllAsync();
    Task<(IEnumerable<Empresa> Items, int TotalCount)> GetAllPagedAsync(
        int page = 1,
        int pageSize = 10,
        string? orderBy = "Id",
        string? direction = "ASC",
        string? searchTerm = null);
    Task<Empresa?> GetByIdAsync(long id);
    Task CreateAsync(Empresa Empresa);
    Task UpdateAsync(Empresa Empresa);
    Task DeleteAsync(Empresa Empresa);
}