using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Empresas.Commands.Create;
using AspNetCore_Condominio.Application.Features.Empresas.Commands.Update;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Enums;

namespace AspNetCore_Condominio.Application.Mappings;

public static class EmpresaMappingExtensions
{
    public static Empresa ToEntity(this CreateCommandEmpresa request)
    {
        return new Empresa
        {
            RazaoSocial = request.RazaoSocial,
            Fantasia = request.Fantasia,
            Cnpj = request.Cnpj?.Replace(".", "").Replace("-", "").Replace("/", "") ?? "",
            TipoDeCondominio = request.TipoDeCondominio,
            Nome = request.Nome,
            Celular = request.Celular,
            Telefone = request.Telefone,
            Email = request.Email,
            Senha = !string.IsNullOrWhiteSpace(request.Senha) ? EncryptionHelper.Encrypt(request.Senha) : null,
            Host = request.Host,
            Porta = request.Porta,
            Cep = request.Cep,
            Uf = request.Uf,
            Cidade = request.Cidade,
            Endereco = request.Endereco,
            Bairro = request.Bairro,
            Complemento = request.Complemento,
            DataInclusao = DateTime.Now,
            Ativo = TipoEmpresaAtivo.Ativo
        };
    }

    public static bool UpdateFromCommand(this Empresa empresa, UpdateCommandEmpresa request)
    {
        bool statusMudouParaInativo = empresa.Ativo == TipoEmpresaAtivo.Ativo && request.Ativo != TipoEmpresaAtivo.Ativo;

        if (!string.IsNullOrEmpty(request.Senha))
            empresa.Senha = EncryptionHelper.Encrypt(request.Senha);

        empresa.Ativo = request.Ativo;
        empresa.RazaoSocial = request.RazaoSocial;
        empresa.Fantasia = request.Fantasia;
        empresa.Cnpj = request.Cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
        empresa.TipoDeCondominio = request.TipoDeCondominio;
        empresa.Nome = request.Nome;
        empresa.Celular = request.Celular;
        empresa.Telefone = request.Telefone;
        empresa.Email = request.Email;
        empresa.Host = request.Host;
        empresa.Porta = request.Porta;
        empresa.Cep = request.Cep;
        empresa.Uf = request.Uf;
        empresa.Cidade = request.Cidade;
        empresa.Endereco = request.Endereco;
        empresa.Bairro = request.Bairro;
        empresa.Complemento = request.Complemento;
        empresa.DataAlteracao = DateTime.Now;

        return statusMudouParaInativo;
    }

    public static EmpresaDto ToDto(this Empresa d) => new()
    {
        Id = d.Id,
        Ativo = d.Ativo,
        RazaoSocial = d.RazaoSocial,
        Fantasia = d.Fantasia,
        Cnpj = d.Cnpj,
        TipoDeCondominio = d.TipoDeCondominio,
        Nome = d.Nome,
        Celular = d.Celular,
        Telefone = d.Telefone ?? "",
        Email = d.Email,
        Senha = null,
        Host = d.Host,
        Porta = d.Porta,
        Cep = d.Cep,
        Uf = d.Uf,
        Cidade = d.Cidade,
        Endereco = d.Endereco,
        Bairro = d.Bairro,
        Complemento = d.Complemento,
        DataInclusao = d.DataInclusao,
        DataAlteracao = d.DataAlteracao
    };
}