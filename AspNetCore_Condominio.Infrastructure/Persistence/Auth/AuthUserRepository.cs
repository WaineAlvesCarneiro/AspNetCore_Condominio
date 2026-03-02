using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using AspNetCore_Condominio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore_Condominio.Infrastructure.Repositories.Auth;

public class AuthUserRepository(ApplicationDbContext context) : IAuthUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<AuthUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.AuthUsers.FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
    }

    public async Task<IEnumerable<AuthUser>> GetAllAsync(long? empresaId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.AuthUsers.AsNoTracking();

        if (empresaId.HasValue && empresaId.Value != 0)
            query = query.Where(u => u.EmpresaId == empresaId.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<AuthUser> Items, int TotalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        long? empresaId, string? userName, CancellationToken cancellationToken = default)
    {
        var query = _context.AuthUsers.AsQueryable();

        if (empresaId.HasValue && empresaId != 0)
            query = query.Where(u => u.EmpresaId == empresaId.Value);

        if (!string.IsNullOrWhiteSpace(userName))
            query = query.Where(x => x.UserName.Contains(userName));

        query = ApplyOrdering(query, orderBy!, direction!);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

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

    public async Task<AuthUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<AuthUser>().AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task CreateAsync(AuthUser authUser, CancellationToken cancellationToken = default)
    {
        await _context.Set<AuthUser>().AddAsync(authUser, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AuthUser authUser, CancellationToken cancellationToken = default)
    {
        _context.Set<AuthUser>().Update(authUser);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(AuthUser authUser, CancellationToken cancellationToken = default)
    {
        _context.Set<AuthUser>().Remove(authUser);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuthUser>> GetByEmpresaIdAsync(long empresaId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<AuthUser>()
            .Where(x => x.EmpresaId == empresaId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteUsuarioVinculadoNaEmpresaAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.AuthUsers.AsNoTracking().AnyAsync(m => m.EmpresaId == id, cancellationToken);
    }
}