namespace AspNetCore_Condominio.Application.DTOs;

public record ImovelDto
{
    public long Id { get; set; }
    public required string Bloco { get; set; }
    public required string Apartamento { get; set; }
    public required string BoxGaragem { get; set; }
    public long EmpresaId { get; set; }
    public EmpresaDto? EmpresaDto { get; set; }
}
