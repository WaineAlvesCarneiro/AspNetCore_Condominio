namespace AspNetCore_Condominio.Domain.Entities;

public class Imovel
{
    public long Id { get; set; }
    public required string Bloco { get; set; }
    public required string Apartamento { get; set; }
    public required string BoxGaragem { get; set; }
    public long EmpresaId { get; set; }

    private readonly List<Morador> _moradores = new();
    public IReadOnlyCollection<Morador> Moradores => _moradores.AsReadOnly();

    public void AdicionarMorador(Morador morador)
    {
        if (morador == null)
        {
            throw new ArgumentNullException(nameof(morador));
        }
        if (_moradores.Count >= 5)
        {
            throw new InvalidOperationException("Limite de 5 moradores atingido para este imóvel.");
        }

        _moradores.Add(morador);
    }
}