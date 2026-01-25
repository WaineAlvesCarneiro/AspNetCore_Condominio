namespace AspNetCore_Condominio.Domain.Entities.Auth;

public class AuthUser
{
    public Guid Id { get; set; }
    public long EmpresaId { get; set; }
    public required string UserName { get; set; }
    public required string PasswordHash { get; set; }
    public required string Role { get; set; }

    public bool VerificarSenha(string senhaBruta)
    {
        return BCrypt.Net.BCrypt.Verify(senhaBruta, this.PasswordHash);
    }
}
