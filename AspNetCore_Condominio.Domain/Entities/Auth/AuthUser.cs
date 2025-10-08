namespace AspNetCore_Condominio.Domain.Entities.Auth;

public class AuthUser
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public string? Role { get; set; }

    public bool VerificarSenha(string senhaBruta)
    {
        return BCrypt.Net.BCrypt.Verify(senhaBruta, this.PasswordHash);
    }
}
