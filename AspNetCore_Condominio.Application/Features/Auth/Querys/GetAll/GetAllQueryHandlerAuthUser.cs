using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Queries.GetAll;

public class GetAllQueryHandlerAuthUser(IAuthUserRepository repository)
    : IRequestHandler<GetAllQueryAuthUser, Result<IEnumerable<AuthUserDto>>>
{
    public async Task<Result<IEnumerable<AuthUserDto>>> Handle(GetAllQueryAuthUser request, CancellationToken cancellationToken)
    {
        var dados = await repository.GetAllAsync();

        var dtos = dados.Select(dado => new AuthUserDto
        {
            Id = dado.Id,
            EmpresaId = dado.EmpresaId,
            UserName = dado.UserName,
            Email = dado.Email,
            Role = dado.Role,
            DataInclusao = dado.DataInclusao,
            DataAlteracao = dado.DataAlteracao
        });

        return Result<IEnumerable<AuthUserDto>>.Success(dtos);
    }
}
