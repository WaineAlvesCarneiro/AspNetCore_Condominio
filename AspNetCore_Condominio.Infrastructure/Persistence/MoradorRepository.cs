using AspNetCore_Condominio.Infrastructure.Data;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AspNetCore_Condominio.Infrastructure.Repositories;

public class MoradorRepository(ApplicationDbContext context) : IMoradorRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Morador>> GetAllAsync()
    {
        return await _context.Set<Morador>()
            .Include(m => m.Imovel)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Morador> Items, int TotalCount)> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction)
    {
        var query = _context.Set<Morador>()
            .Include(m => m.Imovel)
            .AsQueryable();

        Expression<Func<Morador, object>> keySelector = orderBy.ToLower() switch
        {
            "id" => i => i.Id,
            "nome" => i => i.Nome,
            "celular" => i => i.Celular,
            "email" => i => i.Email,
            "isproprietario" => i => i.IsProprietario,
            "dataentrada" => i => i.DataEntrada,
            "datasaida" => i => i.DataSaida,
            "datainclusao" => i => i.DataInclusao,
            "dataalteracao" => i => i.DataAlteracao,
            "imovelid" => i => i.ImovelId,
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

    public async Task<Morador?> GetByIdAsync(int id)
    {
        return await _context.Set<Morador>()
            .Include(m => m.Imovel)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
    public async Task CreateAsync(Morador morador)
    {
        await _context.Set<Morador>().AddAsync(morador);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Morador morador)
    {
        _context.Set<Morador>().Update(morador);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Morador morador)
    {
        _context.Set<Morador>().Remove(morador);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByImovelIdAsync(int imovelId)
    {
        return await _context.Moradors.AnyAsync(m => m.ImovelId == imovelId);
    }
}