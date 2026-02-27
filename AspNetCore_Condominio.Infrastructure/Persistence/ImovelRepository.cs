using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore_Condominio.Infrastructure.Repositories;

public class ImovelRepository(ApplicationDbContext context) : IImovelRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Imovel>> GetAllAsync(long? empresaId = null)
    {
        var query = _context.Imovels.AsNoTracking();
        if (empresaId.HasValue && empresaId.Value != 0)
            query = query.Where(u => u.EmpresaId == empresaId.Value);
        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<Imovel> items, int totalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        long? empresaId, string? bloco, string? apartamento)
    {
        var query = _context.Imovels.AsQueryable();

        if (empresaId.HasValue && empresaId != 0)
            query = query.Where(u => u.EmpresaId == empresaId.Value);

        if (!string.IsNullOrWhiteSpace(bloco))
            query = query.Where(x => x.Bloco.Contains(bloco));

        if (!string.IsNullOrWhiteSpace(apartamento))
            query = query.Where(x => x.Apartamento.Contains(apartamento));

        query = ApplyOrdering(query, orderBy!, direction!);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (items, totalCount);
    }

    private IQueryable<Imovel> ApplyOrdering(IQueryable<Imovel> query, string orderBy, string direction)
        {
        direction = direction?.ToUpper() ?? "ASC";

        return (orderBy?.ToLower(), direction) switch
        {
            ("bloco", "ASC") => query.OrderBy(m => m.Bloco),
            ("bloco", "DESC") => query.OrderByDescending(m => m.Bloco),
            ("apartamento", "ASC") => query.OrderBy(m => m.Apartamento),
            ("apartamento", "DESC") => query.OrderByDescending(m => m.Apartamento),
            ("boxgaragem", "ASC") => query.OrderBy(m => m.BoxGaragem),
            ("boxgaragem", "DESC") => query.OrderByDescending(m => m.BoxGaragem),
            (_, "DESC") => query.OrderByDescending(m => m.Id),
            _ => query.OrderBy(m => m.Id)
        };
    }

    public async Task<Imovel?> GetByIdAsync(long id)
    {
        return await _context.Imovels
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task CreateAsync(Imovel imovel)
    {
        await _context.Set<Imovel>().AddAsync(imovel);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Imovel imovel)
    {
        _context.Set<Imovel>().Update(imovel);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Imovel imovel)
    {
        _context.Set<Imovel>().Remove(imovel);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteImovelVinculadoNaEmpresaAsync(long userEmpresaId)
    {
        return await _context.Imovels.AnyAsync(m => m.EmpresaId == userEmpresaId);
    }
}
