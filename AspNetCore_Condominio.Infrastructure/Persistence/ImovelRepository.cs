using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AspNetCore_Condominio.Infrastructure.Repositories;

public class ImovelRepository(ApplicationDbContext context) : IImovelRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Imovel>> GetAllAsync()
    {
        return await _context.Set<Imovel>().ToListAsync();
    }

    public async Task<(IEnumerable<Imovel> Items, int TotalCount)> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction)
    {
        var query = _context.Set<Imovel>().AsQueryable();

        Expression<Func<Imovel, object>> keySelector = orderBy.ToLower() switch
        {
            "id" => i => i.Id,
            "bloco" => i => i.Bloco,
            "apartamento" => i => i.Apartamento,
            "box_garagem" => i => i.BoxGaragem,
            _ => throw new NotImplementedException()
        };

        query = direction.ToLower() == "desc" ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);

        var items = await query
            .Skip((page) * linesPerPage)
            .Take(linesPerPage)
            .ToListAsync();

        var totalCount = await query.CountAsync();

        return (items, totalCount);
    }

    public async Task<Imovel?> GetByIdAsync(int id)
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
}
