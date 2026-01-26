using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Queries.GetById;

public class GetByIdQueryHandlerEmpresa(IEmpresaRepository repository)
    : IRequestHandler<GetByIdQueryEmpresa, Result<EmpresaDto>>
{
    public async Task<Result<EmpresaDto>> Handle(GetByIdQueryEmpresa request, CancellationToken cancellationToken)
    {
        var dado = await repository.GetByIdAsync(request.Id);
        if (dado is null)
            return Result<EmpresaDto>.Failure("Empresa não encontrada.");

        var dto = new EmpresaDto
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
        };

        return Result<EmpresaDto>.Success(dto);
    }
}