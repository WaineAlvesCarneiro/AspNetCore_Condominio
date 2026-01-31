using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.DefinirSenha;

public class DefinirSenhaCommandHandler(IAuthUserRepository repository)
    : IRequestHandler<DefinirSenhaCommand, Result<AuthUserDto>>
{
    private readonly IAuthUserRepository _repository = repository;

    public async Task<Result<AuthUserDto>> Handle(DefinirSenhaCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _repository.GetByUsernameAsync(request.UserName);

        if (usuario == null)
            return Result<AuthUserDto>.Failure("Usuário não encontrado.");

        usuario.PasswordHash = PasswordHasher.HashPassword(request.NovaSenha);
        usuario.PrimeiroAcesso = false;
        usuario.DataAlteracao = DateTime.Now;

        await _repository.UpdateAsync(usuario);

        var dto = new AuthUserDto
        {
            Id = usuario.Id,
            EmpresaId = usuario.EmpresaId,
            UserName = usuario.UserName,
            Email = usuario.Email,
            PrimeiroAcesso = usuario.PrimeiroAcesso,
            Role = usuario.Role,
            DataInclusao = usuario.DataInclusao,
            DataAlteracao = usuario.DataAlteracao
        };

        return Result<AuthUserDto>.Success(dto, "Senha definida com sucesso!");
    }
}