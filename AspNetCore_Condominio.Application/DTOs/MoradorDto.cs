namespace AspNetCore_Condominio.Application.DTOs;

public class MoradorDto
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Celular { get; set; }
    public required string Email { get; set; }
    public bool IsProprietario { get; set; }
    public DateOnly DataEntrada { get; set; }
    public DateOnly? DataSaida { get; set; }
    public DateOnly DataInclusao { get; set; }
    public DateOnly? DataAlteracao { get; set; }
    public int ImovelId { get; set; }
    public ImovelDto? ImovelDto { get; set; }
}