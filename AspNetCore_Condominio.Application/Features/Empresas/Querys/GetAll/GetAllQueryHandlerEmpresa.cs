using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Queries.GetAll;

public class GetAllQueryHandlerEmpresa(IEmpresaRepository repository)
    : IRequestHandler<GetAllQueryEmpresa, Result<IEnumerable<EmpresaDto>>>
{
    public async Task<Result<IEnumerable<EmpresaDto>>> Handle(GetAllQueryEmpresa request, CancellationToken cancellationToken)
    {
        var dados = await repository.GetAllAsync();

        var dtos = dados.Select(item => new EmpresaDto
        {
            Id = item.Id,
            RazaoSocial = item.RazaoSocial,
            Fantasia = item.Fantasia,
            Cnpj = item.Cnpj,
            TipoDeCondominio = item.TipoDeCondominio,
            Nome = item.Nome,
            Celular = item.Celular,
            Telefone = item.Telefone!,
            Email = item.Email,
            Cep = item.Cep,
            Uf = item.Uf,
            Cidade = item.Cidade,
            Endereco = item.Endereco,
            Complemento = item.Complemento,
            DataInclusao = item.DataInclusao,
            DataAlteracao = item.DataAlteracao
        });

        return Result<IEnumerable<EmpresaDto>>.Success(dtos);
    }
}
