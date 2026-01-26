using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Interfaces;

namespace AspNetCore_Condominio.Domain.Entities;

public class Empresa : IEntidadeComEmail
{
    public long Id { get; set; }
    public required string RazaoSocial { get; set; }
    public required string Fantasia { get; set; }
    public required string Cnpj { get; set; }
    public TipoCondominio TipoDeCondominio { get; set; }
    public required string Nome { get; set; }
    public required string Celular { get; set; }
    public string? Telefone { get; set; }
    public required string Email { get; set; } = null!;
    public string? Senha { get; set; }
    public required string Host { get; set; }
    public int Porta { get; set; }
    public required string Cep { get; set; }
    public required string Uf { get; set; }
    public required string Cidade { get; set; }
    public required string Endereco { get; set; }
    public string? Complemento { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataAlteracao { get; set; }
}