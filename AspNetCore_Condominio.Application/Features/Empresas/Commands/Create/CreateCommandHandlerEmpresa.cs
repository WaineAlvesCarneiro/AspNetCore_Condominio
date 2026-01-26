using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Events;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Create;

public record CreateCommandHandlerEmpresa(IEmpresaRepository repository, IMediator mediator)
    : IRequestHandler<CreateCommandEmpresa, Result<EmpresaDto>>
{
    public async Task<Result<EmpresaDto>> Handle(CreateCommandEmpresa request, CancellationToken cancellationToken)
    {
        var dado = new Empresa
        {
            RazaoSocial = request.RazaoSocial,
            Fantasia = request.Fantasia,
            Cnpj = request.Cnpj,
            TipoDeCondominio = request.TipoDeCondominio,
            Nome = request.Nome,
            Celular = request.Celular,
            Telefone = request.Telefone,
            Email = request.Email,
            Senha = request.Senha,
            Host = request.Host,
            Porta = request.Porta,
            Cep = request.Cep,
            Uf = request.Uf,
            Cidade = request.Cidade,
            Endereco = request.Endereco,
            Complemento = request.Complemento,
            DataInclusao = request.DataInclusao
        };

        await repository.CreateAsync(dado);

        await mediator.Publish(new CriadoEventEmail<Empresa>(dado, true), cancellationToken);

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

        return Result<EmpresaDto>.Success(dto, "Empresa criada com sucesso.");
    }
}