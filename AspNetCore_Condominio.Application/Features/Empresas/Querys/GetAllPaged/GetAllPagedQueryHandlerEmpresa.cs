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

        IEnumerable<EmpresaDto> dtos = items.Select(dado => new EmpresaDto
        {
            Id = dado.Id,
            RazaoSocial = dado.RazaoSocial,
            Fantasia = dado.Fantasia,
            Cnpj = dado.Cnpj,
            TipoDeCondominio = dado.TipoDeCondominio,
            Nome = dado.Nome,
            Celular = dado.Celular,
            Telefone = dado.Telefone!,
            Email = dado.Email,
            Senha = dado.Senha,
            Host = dado.Host,
            Porta = dado.Porta,
            Cep = dado.Cep,
            Uf = dado.Uf,
            Cidade = dado.Cidade,
            Endereco = dado.Endereco,
            Complemento = dado.Complemento,
            DataInclusao = dado.DataInclusao,
            DataAlteracao = dado.DataAlteracao
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
