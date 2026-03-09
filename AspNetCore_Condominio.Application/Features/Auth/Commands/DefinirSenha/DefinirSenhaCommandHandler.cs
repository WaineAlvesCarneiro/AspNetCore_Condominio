using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.DefinirSenha;

public class DefinirSenhaCommandHandler(IAuthUserRepository repository)
    : IRequestHandler<DefinirSenhaCommand, Result<AuthUserDto>>
{
    public async Task<Result<AuthUserDto>> Handle(DefinirSenhaCommand request, CancellationToken cancellationToken)
    {
        var usuario = await repository.GetByUsernameAsync(request.UserName, cancellationToken);

        if (usuario == null)
            return Result<AuthUserDto>.Failure("Usuário não encontrado.");

        usuario.PasswordHash = PasswordHasher.HashPassword(request.NovaSenha);
        usuario.PrimeiroAcesso = false;
        usuario.DataAlteracao = DateTime.Now;

        await repository.UpdateAsync(usuario);

        return Result<AuthUserDto>.Success(usuario.ToDto(), "Senha definida com sucesso!");
    }
}