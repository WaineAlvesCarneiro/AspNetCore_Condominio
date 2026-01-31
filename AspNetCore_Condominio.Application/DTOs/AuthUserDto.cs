using AspNetCore_Condominio.Domain.Enums;

namespace AspNetCore_Condominio.Application.DTOs;

public record AuthUserDto
{
    public Guid Id { get; set; }
    public long? EmpresaId { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public bool PrimeiroAcesso { get; set; } = true;
    public TipoRole Role { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataAlteracao { get; set; }
}
