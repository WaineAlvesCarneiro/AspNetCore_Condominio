namespace AspNetCore_Condominio.Application.DTOs;

public class MensagemEmailDto
{
    public long EmpresaId { get; set; }
    public string Para { get; set; } = string.Empty;
    public string Assunto { get; set; } = string.Empty;
    public string Corpo { get; set; } = string.Empty;
}