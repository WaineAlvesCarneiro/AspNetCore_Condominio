using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Queries.GetAllPaged;

public class GetAllPagedQueryHandlerEmpresa(IEmpresaRepository repository)
    : IRequestHandler<GetAllPagedQueryEmpresa, Result<PagedResult<EmpresaDto>>>
{
    private readonly IEmpresaRepository _repository = repository;

    public async Task<Result<PagedResult<EmpresaDto>>> Handle(
        GetAllPagedQueryEmpresa request,
        CancellationToken cancellationToken)
    {
        (IEnumerable<Empresa> items, int totalCount) = await _repository.GetAllPagedAsync(
            page: request.ActualPage,
            pageSize: request.ActualPageSize,
            orderBy: request.ActualSortBy,
            direction: request.ActualDirection,
            searchTerm: request.SearchTerm
        );

        IEnumerable<EmpresaDto> dtos = items.Select(i => new EmpresaDto
        {
            Id = i.Id,
            RazaoSocial = i.RazaoSocial,
            Fantasia = i.Fantasia,
            Cnpj = i.Cnpj,
            TipoDeCondominio = i.TipoDeCondominio,
            Nome = i.Nome,
            Celular = i.Celular,
            Telefone = i.Telefone!,
            Email = i.Email,
            Cep = i.Cep,
            Uf = i.Uf,
            Cidade = i.Cidade,
            Endereco = i.Endereco,
            Complemento = i.Complemento,
            DataInclusao = i.DataInclusao,
            DataAlteracao = i.DataAlteracao
        });

        PagedResult<EmpresaDto> pagedResult = new PagedResult<EmpresaDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = request.ActualPage,
            LinesPerPage = request.ActualPageSize
        };

        return Result<PagedResult<EmpresaDto>>.Success(pagedResult);
    }
}
