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
        var users = await _context.AuthUsers.ToListAsync();

        return users.FirstOrDefault(u => string.Equals(u.UserName, username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<AuthUser>> GetAllAsync()
    {
        return await _context.Set<AuthUser>().ToListAsync();
    }

    public async Task<(IEnumerable<AuthUser> Items, int TotalCount)> GetAllPagedAsync(
        int page = 1,
        int pageSize = 10,
        string? orderBy = "Id",
        string? direction = "ASC",
        string? searchTerm = null)
    {
        var query = _context.AuthUsers
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(m =>
                m.UserName.Contains(searchTerm);
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


    private IQueryable<AuthUser> ApplyOrdering(IQueryable<AuthUser> query, string orderBy, string direction)
    {
        direction = direction?.ToUpper() ?? "ASC";

        return (orderBy?.ToLower(), direction) switch
        {
            ("userName", "ASC") => query.OrderBy(m => m.UserName),
            ("userName", "DESC") => query.OrderByDescending(m => m.UserName),
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
}