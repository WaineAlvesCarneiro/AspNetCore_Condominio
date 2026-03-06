using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Update;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.Update;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;

namespace AspNetCore_Condominio.Application.Mappings;

public static class AuthUserMappingExtensions
{
    public static AuthUser ToEntity(this CreateCommandAuthUser request, string senhaTemporaria)
    {
        return new AuthUser
        {
            EmpresaId = request.EmpresaId,
            UserName = request.UserName,
            Email = request.Email,
            PrimeiroAcesso = true,
            PasswordHash = PasswordHasher.HashPassword(senhaTemporaria),
            Role = request.Role,
            DataInclusao = DateTime.Now,
            Ativo = TipoUserAtivo.Ativo,
            EmpresaAtiva = TipoEmpresaAtivo.Ativo
        };
    }

    public static void UpdateFromCommand(this AuthUser entidade, UpdateCommandAuthUser request)
    {
        entidade.UserName = request.UserName;
        entidade.Email = request.Email;
        entidade.Role = (TipoRole)request.Role!;
        entidade.Ativo = request.Ativo;
        entidade.EmpresaAtiva = request.EmpresaAtiva;
        entidade.DataAlteracao = DateTime.Now;
    }

    public static AuthUserDto ToDto(this AuthUser dado) => new()
    {
        Id = dado.Id,
        Ativo = dado.Ativo,
        EmpresaAtiva = dado.EmpresaAtiva,
        EmpresaId = dado.EmpresaId,
        UserName = dado.UserName,
        Email = dado.Email,
        PrimeiroAcesso = dado.PrimeiroAcesso,
        Role = dado.Role,
        DataInclusao = dado.DataInclusao,
        DataAlteracao = dado.DataAlteracao
    };
}