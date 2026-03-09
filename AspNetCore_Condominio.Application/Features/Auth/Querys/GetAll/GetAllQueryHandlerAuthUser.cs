using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Queries.GetAll;

public class GetAllQueryHandlerAuthUser(IAuthUserRepository repository)
    : IRequestHandler<GetAllQueryAuthUser, Result<IEnumerable<AuthUserDto>>>
{
    public async Task<Result<IEnumerable<AuthUserDto>>> Handle(GetAllQueryAuthUser request, CancellationToken cancellationToken)
    {
        var dados = await repository.GetAllAsync(
            empresaId: request.IdEmpresa, cancellationToken);

        var dtos = dados.Select(dado => dado.ToDto()).ToList();

        return Result<IEnumerable<AuthUserDto>>.Success(dtos);
    }
}
