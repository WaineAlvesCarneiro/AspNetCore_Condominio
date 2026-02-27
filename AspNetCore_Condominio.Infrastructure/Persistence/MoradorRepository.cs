using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore_Condominio.Infrastructure.Repositories;

public class MoradorRepository(ApplicationDbContext context) : IMoradorRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Morador>> GetAllAsync(long? empresaId = null)
    {
        var query = _context.Moradores
            .Include(m => m.Imovel)
            .Include(m => m.Empresa)
            .AsQueryable();

        if (empresaId.HasValue && empresaId.Value != 0)
            query = query.Where(u => u.EmpresaId == empresaId.Value);

        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<Morador> items, int totalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        long? empresaId, string? nome)
    {
        var query = _context.Moradores
            .Include(m => m.Imovel)
            .Include(m => m.Empresa)
            .AsQueryable();

        if (empresaId.HasValue && empresaId != 0)
            query = query.Where(u => u.EmpresaId == empresaId.Value);

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(x => x.Nome.Contains(nome));

        query = ApplyOrdering(query, orderBy!, direction!);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (items, totalCount);
    }

    private IQueryable<Morador> ApplyOrdering(IQueryable<Morador> query, string orderBy, string direction)
    {
        direction = direction?.ToUpper() ?? "ASC";

        return (orderBy?.ToLower(), direction) switch
        {
            ("nome", "ASC") => query.OrderBy(m => m.Nome),
            ("nome", "DESC") => query.OrderByDescending(m => m.Nome),
            ("email", "ASC") => query.OrderBy(m => m.Email),
            ("email", "DESC") => query.OrderByDescending(m => m.Email),
            ("datainclusao", "ASC") => query.OrderBy(m => m.DataInclusao),
            ("datainclusao", "DESC") => query.OrderByDescending(m => m.DataInclusao),
            (_, "DESC") => query.OrderByDescending(m => m.Id),
            _ => query.OrderBy(m => m.Id)
        };
    }

    public async Task<Morador?> GetByIdAsync(long id)
    {
        return await _context.Moradores
            .Include(m => m.Imovel)
            .Include(m => m.Empresa)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task CreateAsync(Morador morador)
    {
        await _context.Set<Morador>().AddAsync(morador);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Morador morador)
    {
        _context.Set<Morador>().Entry(morador).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Morador morador)
    {
        _context.Set<Morador>().Remove(morador);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteMoradorVinculadoNoImovelAsync(long imovelId)
    {
        return await _context.Moradores.AnyAsync(m => m.ImovelId == imovelId);
    }
}