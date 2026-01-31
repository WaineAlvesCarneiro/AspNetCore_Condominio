using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.DefinirSenha;

public class DefinirSenhaCommand : IRequest<Result<AuthUserDto>>
{
    public string UserName { get; set; } = string.Empty;
    public string NovaSenha { get; set; } = string.Empty;
}
