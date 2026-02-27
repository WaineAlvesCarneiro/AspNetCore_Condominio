using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore_Condominio.Infrastructure.Repositories;

public class EmpresaRepository(ApplicationDbContext context) : IEmpresaRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Empresa>> GetAllAsync(long? empresaId = null)
    {
        var query = _context.Empresas.AsNoTracking();
        if (empresaId.HasValue && empresaId.Value != 0)
            query = query.Where(u => u.Id == empresaId.Value);
        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<Empresa> Items, int TotalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
       string? razaoSocial, string? cnpj)
    {
        var query = _context.Empresas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(razaoSocial))
            query = query.Where(x => x.RazaoSocial.Contains(razaoSocial));

        if (!string.IsNullOrWhiteSpace(cnpj))
            query = query.Where(x => x.Cnpj.Contains(cnpj));

        query = ApplyOrdering(query, orderBy!, direction!);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (items, totalCount);
    }


    private IQueryable<Empresa> ApplyOrdering(IQueryable<Empresa> query, string orderBy, string direction)
    {
        direction = direction?.ToUpper() ?? "ASC";

        return (orderBy?.ToLower(), direction) switch
        {
            ("razaoSocial", "ASC") => query.OrderBy(m => m.RazaoSocial),
            ("razaoSocial", "DESC") => query.OrderByDescending(m => m.RazaoSocial),
            ("cnpj", "ASC") => query.OrderBy(m => m.Cnpj),
            ("cnpj", "DESC") => query.OrderByDescending(m => m.Cnpj),
            (_, "DESC") => query.OrderByDescending(m => m.Id),
            _ => query.OrderBy(m => m.Id)
        };
    }

    public async Task<Empresa?> GetByIdAsync(long id)
    {
        return await _context.Set<Empresa>().FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task CreateAsync(Empresa Empresa)
    {
        await _context.Set<Empresa>().AddAsync(Empresa);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Empresa Empresa)
    {
        _context.Set<Empresa>().Update(Empresa);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Empresa Empresa)
    {
        _context.Set<Empresa>().Remove(Empresa);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteCnpjAsync(string cnpj, long id, CancellationToken cancellation)
    {
        return await _context.Empresas
            .AsNoTracking()
            .AnyAsync(x => x.Cnpj == cnpj && x.Id != id, cancellation);
    }
}
