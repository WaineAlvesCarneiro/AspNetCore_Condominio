using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using AspNetCore_Condominio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore_Condominio.Infrastructure.Repositories.Auth;

public class AuthUserRepository(ApplicationDbContext context) : IAuthUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<AuthUser> GetByUsernameAsync(string username)
    {
        return await _context.AuthUsers
                    .FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<IEnumerable<AuthUser>> GetAllAsync(long? empresaId = null)
    {
        var query = _context.AuthUsers.AsNoTracking();
        if (empresaId.HasValue && empresaId.Value != 0)
            query = query.Where(u => u.EmpresaId == empresaId.Value);
        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<AuthUser> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize, string? orderBy, string? direction,
        long? empresaId, string? userName)
    {
        var query = _context.AuthUsers.AsQueryable();

        if (empresaId.HasValue && empresaId != 0)
            query = query.Where(u => u.EmpresaId == empresaId.Value);

        if (!string.IsNullOrWhiteSpace(userName))
            query = query.Where(x => x.UserName.Contains(userName));

        query = ApplyOrdering(query, orderBy!, direction!);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (items, totalCount);
    }


    private IQueryable<AuthUser> ApplyOrdering(IQueryable<AuthUser> query, string orderBy, string direction)
    {
        direction = direction?.ToUpper() ?? "ASC";

        return (orderBy?.ToLower(), direction) switch
        {
            ("username", "asc") => query.OrderBy(m => m.UserName),
            ("username", "desc") => query.OrderByDescending(m => m.UserName),
            ("email", "asc") => query.OrderBy(m => m.Email),
            ("email", "desc") => query.OrderByDescending(m => m.Email),
            (_, "DESC") => query.OrderByDescending(m => m.Id),
            _ => query.OrderBy(m => m.Id)
        };
    }

    public async Task<AuthUser?> GetByIdAsync(Guid id)
    {
        return await _context.Set<AuthUser>().FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task CreateAsync(AuthUser AuthUser)
    {
        await _context.Set<AuthUser>().AddAsync(AuthUser);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AuthUser AuthUser)
    {
        _context.Set<AuthUser>().Update(AuthUser);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(AuthUser AuthUser)
    {
        _context.Set<AuthUser>().Remove(AuthUser);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuthUser>> GetByEmpresaIdAsync(long empresaId)
    {
        return await _context.Set<AuthUser>()
            .Where(x => x.EmpresaId == empresaId)
            .ToListAsync();
    }

    public async Task<bool> ExisteUsuarioVinculadoNaEmpresaAsync(long id)
    {
        return await _context.AuthUsers.AnyAsync(m => m.EmpresaId == id);
    }
}