using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore_Condominio.Infrastructure.Repositories;

public class ImovelRepository(ApplicationDbContext context) : IImovelRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Imovel>> GetAllAsync()
    {
        return await _context.Set<Imovel>().ToListAsync();
    }

    public async Task<(IEnumerable<Imovel> items, int totalCount)> GetAllPagedAsync(
        int page = 1,
        int pageSize = 10,
        string? orderBy = "Id",
        string? direction = "ASC",
        string? searchTerm = null)
    {
        var query = _context.Imovels
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(m =>
                m.Bloco.Contains(searchTerm) ||
                m.Apartamento.Contains(searchTerm) ||
                m.BoxGaragem.Contains(searchTerm));
        }

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
        return await _context.Set<Imovel>().FirstOrDefaultAsync(i => i.Id == id);
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

    public async Task<bool> ExistsImovelVinculadoNaEmpresaAsync(long empresaId)
    {
        return await _context.Imovels.AnyAsync(m => m.EmpresaId == empresaId);
    }
}
