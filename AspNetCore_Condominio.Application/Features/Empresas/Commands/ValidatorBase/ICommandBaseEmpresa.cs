using AspNetCore_Condominio.Domain.Enums;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.ValidatorBase;

public interface ICommandBaseEmpresa
{
    string RazaoSocial { get; set; }
    string Fantasia { get; set; }
    string Cnpj { get; set; }
    TipoCondominio TipoDeCondominio { get; set; }
    string Nome { get; set; }
    string Celular { get; set; }
    string? Telefone { get; set; }
    string Email { get; set; }
    string Cep { get; set; }
    string Uf { get; set; }
    string Cidade { get; set; }
    string Endereco { get; set; }
    string? Complemento { get; set; }
    DateTime DataInclusao { get; set; }
    DateTime? DataAlteracao { get; set; }
}