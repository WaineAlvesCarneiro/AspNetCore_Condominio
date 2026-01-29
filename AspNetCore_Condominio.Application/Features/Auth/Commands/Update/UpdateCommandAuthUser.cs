using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Auth.Commands.ValidatorBase;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Enums;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Update;

public class UpdateCommandAuthUser : IRequest<Result<AuthUserDto>>, ICommandBaseAuthUser
{
    public Guid Id { get; set; }
    public long? EmpresaId { get; set; }
    public required string UserName { get; set; }
    public required string PasswordHash { get; set; }
    public TipoRole Role { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataAlteracao { get; set; }
}