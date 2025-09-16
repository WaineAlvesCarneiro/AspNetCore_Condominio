namespace AspNetCore_Condominio.Domain.Entities;

public class Morador
{
    public int Id { get; set; }
    public required string Nome { get; set; } = null!;
    public required string Celular { get; set; } = null!;
    public required string Email { get; set; } = null!;
    public bool IsProprietario { get; set; }
    public DateTime DataEntrada { get; set; }
    public DateTime? DataSaida { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public int ImovelId { get; set; }
    public Imovel? Imovel { get; set; } = null!;

    public void AlterarEmail(string novoEmail)
    {
        if (!novoEmail.Contains("@"))
        {
            throw new ArgumentException("E-mail inválido.");
        }
        Email = novoEmail;
        DataAlteracao = DateTime.Now;
    }
}