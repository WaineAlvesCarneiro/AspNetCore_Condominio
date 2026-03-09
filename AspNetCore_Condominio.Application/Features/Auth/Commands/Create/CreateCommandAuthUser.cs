using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Auth.Commands.ValidatorBase;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Enums;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Create;

public class CreateCommandAuthUser : IRequest<Result<AuthUserDto>>, ICommandBaseAuthUser
{
    public int Id { get; set; }
    public TipoUserAtivo Ativo { get; set; }
    public TipoEmpresaAtivo EmpresaAtiva { get; set; }
    public long? EmpresaId { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public TipoRole Role { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataAlteracao { get; set; }
}