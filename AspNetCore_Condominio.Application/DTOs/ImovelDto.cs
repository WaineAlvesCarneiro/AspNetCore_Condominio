namespace AspNetCore_Condominio.Application.DTOs;

public class ImovelDto
{
    public int Id { get; set; }
    public required string Bloco { get; set; }
    public required string Apartamento { get; set; }
    public required string BoxGaragem { get; set; }
}
