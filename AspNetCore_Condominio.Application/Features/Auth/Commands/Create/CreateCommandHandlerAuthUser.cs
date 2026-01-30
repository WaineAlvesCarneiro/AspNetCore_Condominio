using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Create;

public record CreateCommandHandlerAuthUser(IAuthUserRepository repository, IMediator mediator)
    : IRequestHandler<CreateCommandAuthUser, Result<AuthUserDto>>
{
    public async Task<Result<AuthUserDto>> Handle(CreateCommandAuthUser request, CancellationToken cancellationToken)
    {
        var dado = new AuthUser
        {
            EmpresaId = request.EmpresaId,
            UserName = request.UserName,
            PasswordHash = PasswordHasher.HashPassword(request.PasswordHash.ToString()),
            Role = request.Role,
            DataInclusao = request.DataInclusao
        };

        await repository.CreateAsync(dado);

        var dto = new AuthUserDto
        {
            Id = dado.Id,
            EmpresaId = dado.EmpresaId,
            UserName = dado.UserName,
            PasswordHash = dado.PasswordHash,
            Role = dado.Role,
            DataInclusao = dado.DataInclusao,
            DataAlteracao = dado.DataAlteracao
        };

        return Result<AuthUserDto>.Success(dto, "Usuário criado com sucesso.");
    }
}