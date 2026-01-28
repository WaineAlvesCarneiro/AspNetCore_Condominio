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

        var dtos = dados.Select(dado => new EmpresaDto
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
            Bairro = dado.Bairro,
            Complemento = dado.Complemento,
            DataInclusao = dado.DataInclusao,
            DataAlteracao = dado.DataAlteracao
        });

        return Result<IEnumerable<EmpresaDto>>.Success(dtos);
    }
}
