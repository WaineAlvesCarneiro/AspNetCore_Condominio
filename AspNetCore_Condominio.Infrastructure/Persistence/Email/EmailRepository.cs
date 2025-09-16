using AspNetCore_Condominio.Infrastructure.Data;
using AspNetCore_Condominio.Domain.Entities.EmailRemetente;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore_Condominio.Infrastructure.Repositories.Email;

public class EmailRepository(ApplicationDbContext context)
{
    private readonly ApplicationDbContext _context = context;
    public async Task<EmailRemetente> GetAsync()
    {
        return await _context.Set<EmailRemetente>().FirstOrDefaultAsync();
    }
}