using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Empresas.Commands.ValidatorBase;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Enums;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Create;

public class CreateCommandEmpresa : IRequest<Result<EmpresaDto>>, ICommandBaseEmpresa
{
    public long Id { get; set; }
    public required string RazaoSocial { get; set; }
    public required string Fantasia { get; set; }
    public required string Cnpj { get; set; }
    public TipoCondominio TipoDeCondominio { get; set; }
    public required string Nome { get; set; }
    public required string Celular { get; set; }
    public string? Telefone { get; set; }
    public required string Email { get; set; }
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