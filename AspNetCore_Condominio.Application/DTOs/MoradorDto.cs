namespace AspNetCore_Condominio.Application.DTOs;

public record MoradorDto
{
    public long Id { get; set; }
    public required string Nome { get; set; }
    public required string Celular { get; set; }
    public required string Email { get; set; }
    public bool IsProprietario { get; set; }
    public DateOnly DataEntrada { get; set; }
    public DateOnly? DataSaida { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public long ImovelId { get; set; }
    public ImovelDto? ImovelDto { get; set; }
    public long EmpresaId { get; set; }
    public EmpresaDto? EmpresaDto { get; set; }
}