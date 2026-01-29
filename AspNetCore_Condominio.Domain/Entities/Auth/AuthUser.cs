using AspNetCore_Condominio.Domain.Enums;

namespace AspNetCore_Condominio.Domain.Entities.Auth;

public class AuthUser
{
    public Guid Id { get; set; }
    public long? EmpresaId { get; set; }
    public required string UserName { get; set; }
    public required string PasswordHash { get; set; }
    public TipoRole Role { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataAlteracao { get; set; }

    public bool VerificarSenha(string senhaBruta)
    {
        return BCrypt.Net.BCrypt.Verify(senhaBruta, this.PasswordHash);
    }
}
