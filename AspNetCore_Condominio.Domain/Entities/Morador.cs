using AspNetCore_Condominio.Domain.Interfaces;

namespace AspNetCore_Condominio.Domain.Entities;

public class Morador : IEntidadeComEmail
{
    public long Id { get; set; }
    public required string Nome { get; set; } = null!;
    public required string Celular { get; set; } = null!;
    public required string Email { get; set; } = null!;
    public bool IsProprietario { get; set; }
    public DateOnly DataEntrada { get; set; }
    public DateOnly? DataSaida { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public long ImovelId { get; set; }
    public virtual Imovel? Imovel { get; set; } = null!;
    public long EmpresaId { get; set; }
    public virtual Empresa? Empresa { get; set; }

    public Morador(){}

    public Morador(string nome, string celular, string email, long imovelId, long empresaId,
                   DateOnly dataEntrada, bool isProprietario = false)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.");
        if (!email.Contains("@"))
            throw new ArgumentException("E-mail inválido.");
        if (imovelId <= 0)
            throw new ArgumentException("Imóvel inválido.");
        if (empresaId <= 0)
            throw new ArgumentException("Empresa inválida.");

        Nome = nome;
        Celular = celular;
        Email = email;
        ImovelId = imovelId;
        EmpresaId = empresaId;
        IsProprietario = isProprietario;
        DataEntrada = dataEntrada;
        DataInclusao = DateTime.UtcNow;
        Imovel = null!;
    }

    public void AlterarEmail(string novoEmail)
    {
        if (string.IsNullOrWhiteSpace(novoEmail))
            throw new ArgumentException("O e-mail não pode ser nulo ou vazio.");

        if (!novoEmail.Contains("@"))
            throw new ArgumentException("E-mail inválido.");

        Email = novoEmail;
        DataAlteracao = DateTime.UtcNow;
    }

    public void DefinirDataSaida(DateOnly? dataSaida)
    {
        if (dataSaida.HasValue && dataSaida.Value < DataEntrada)
            throw new ArgumentException("Data de saída não pode ser anterior à data de entrada.");

        DataSaida = dataSaida;
        DataAlteracao = DateTime.UtcNow;
    }

    public bool ValidarDatas()
    {
        return !(DataSaida.HasValue && DataSaida.Value < DataEntrada);
    }

    public bool EstaAtivo()
    {
        return !DataSaida.HasValue || DataSaida.Value >= DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public void AtualizarDados(string nome, string celular, long imovelId, bool isProprietario)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.");

        Nome = nome;
        Celular = celular;
        ImovelId = imovelId;
        IsProprietario = isProprietario;
        DataAlteracao = DateTime.UtcNow;
    }
}