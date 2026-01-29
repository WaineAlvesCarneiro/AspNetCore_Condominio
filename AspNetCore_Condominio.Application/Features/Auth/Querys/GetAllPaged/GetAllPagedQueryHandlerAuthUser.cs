using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Queries.GetAllPaged;

public class GetAllPagedQueryHandlerAuthUser(IAuthUserRepository repository)
    : IRequestHandler<GetAllPagedQueryAuthUser, Result<PagedResult<AuthUserDto>>>
{
    private readonly IAuthUserRepository _repository = repository;

    public async Task<Result<PagedResult<AuthUserDto>>> Handle(
        GetAllPagedQueryAuthUser request,
        CancellationToken cancellationToken)
    {
        (IEnumerable<AuthUser> items, int totalCount) = await _repository.GetAllPagedAsync(
            page: request.ActualPage,
            pageSize: request.ActualPageSize,
            orderBy: request.ActualSortBy,
            direction: request.ActualDirection,
            searchTerm: request.SearchTerm
        );

        IEnumerable<AuthUserDto> dtos = items.Select(dado => new AuthUserDto
        {
            Id = dado.Id,
            EmpresaId = dado.EmpresaId,
            UserName = dado.UserName,
            PasswordHash = dado.PasswordHash,
            Role = dado.Role,
            DataInclusao = dado.DataInclusao,
            DataAlteracao = dado.DataAlteracao
        });

        PagedResult<AuthUserDto> pagedResult = new PagedResult<AuthUserDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = request.ActualPage,
            LinesPerPage = request.ActualPageSize
        };

        return Result<PagedResult<AuthUserDto>>.Success(pagedResult);
    }
}
