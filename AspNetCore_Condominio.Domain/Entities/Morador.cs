namespace AspNetCore_Condominio.Domain.Entities;

public class Morador
{
    public int Id { get; set; }
    public required string Nome { get; set; } = null!;
    public required string Celular { get; set; } = null!;
    public required string Email { get; set; } = null!;
    public bool IsProprietario { get; set; }
    public DateTime DataEntrada { get; set; } = DateTime.Now;
    public DateTime? DataSaida { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.Now;
    public DateTime? DataAlteracao { get; set; }
    public int ImovelId { get; set; }
    public Imovel? Imovel { get; set; } = null!;

    public Morador() { }

    public Morador(string nome, string celular, string email, int imovelId)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome é obrigatório.");
        if (!email.Contains("@")) throw new ArgumentException("E-mail inválido.");

        Nome = nome;
        Celular = celular;
        Email = email;
        ImovelId = imovelId;
        IsProprietario = false;
        DataEntrada = DateTime.Now;
        DataInclusao = DateTime.Now;
    }

    public void AlterarEmail(string novoEmail)
    {
        if (string.IsNullOrWhiteSpace(novoEmail))
        {
            throw new ArgumentException("O e-mail não pode ser nulo ou vazio.");
        }

        if (!novoEmail.Contains("@"))
        {
            throw new ArgumentException("E-mail inválido.");
        }

        Email = novoEmail;
        DataAlteracao = DateTime.Now;
    }
}