namespace AspNetCore_Condominio.Domain.Entities.Auth;

public class AuthUser
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
}
