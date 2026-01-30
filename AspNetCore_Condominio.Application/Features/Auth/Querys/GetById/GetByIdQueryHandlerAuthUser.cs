using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Queries.GetById;

public class GetByIdQueryHandlerAuthUser(IAuthUserRepository repository)
    : IRequestHandler<GetByIdQueryAuthUser, Result<AuthUserDto>>
{
    public async Task<Result<AuthUserDto>> Handle(GetByIdQueryAuthUser request, CancellationToken cancellationToken)
    {
        var dado = await repository.GetByIdAsync(request.Id);
        if (dado is null)
            return Result<AuthUserDto>.Failure("Usuário não encontrado.");

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

        return Result<AuthUserDto>.Success(dto);
    }
}