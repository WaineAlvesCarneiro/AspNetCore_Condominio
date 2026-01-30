using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Update;

public record UpdateCommandHandlerEmpresa(IEmpresaRepository repository)
    : IRequestHandler<UpdateCommandEmpresa, Result<EmpresaDto>>
{
    private readonly IEmpresaRepository _repository = repository;

    public async Task<Result<EmpresaDto>> Handle(UpdateCommandEmpresa request, CancellationToken cancellationToken)
    {
        var dadoToUpdate = await _repository.GetByIdAsync(request.Id);
        if (dadoToUpdate == null)
        {
            return Result<EmpresaDto>.Failure("Empresa não encontrado.");
        }

        dadoToUpdate.RazaoSocial = request.RazaoSocial;
        dadoToUpdate.Fantasia = request.Fantasia;
        dadoToUpdate.Cnpj = request.Cnpj;
        dadoToUpdate.TipoDeCondominio = request.TipoDeCondominio;
        dadoToUpdate.Nome = request.Nome;
        dadoToUpdate.Celular = request.Celular;
        dadoToUpdate.Telefone = request.Telefone;
        dadoToUpdate.Email = request.Email;
        dadoToUpdate.Senha = dadoToUpdate.Senha;
        dadoToUpdate.Host = dadoToUpdate.Host;
        dadoToUpdate.Porta = dadoToUpdate.Porta;
        dadoToUpdate.Cep = request.Cep;
        dadoToUpdate.Uf = request.Uf;
        dadoToUpdate.Cidade = request.Cidade;
        dadoToUpdate.Endereco = request.Endereco;
        dadoToUpdate.Bairro = request.Bairro;
        dadoToUpdate.Complemento = request.Complemento;
        dadoToUpdate.DataInclusao = request.DataInclusao;
        dadoToUpdate.DataAlteracao = request.DataAlteracao;

        await _repository.UpdateAsync(dadoToUpdate);

        var dto = new EmpresaDto
        {
            Id = dadoToUpdate.Id,
            RazaoSocial = dadoToUpdate.RazaoSocial,
            Fantasia = dadoToUpdate.Fantasia,
            Cnpj = dadoToUpdate.Cnpj,
            TipoDeCondominio = dadoToUpdate.TipoDeCondominio,
            Nome = dadoToUpdate.Nome,
            Celular = dadoToUpdate.Celular,
            Telefone = dadoToUpdate.Telefone!,
            Email = dadoToUpdate.Email,
            Senha = dadoToUpdate.Senha,
            Host = dadoToUpdate.Host,
            Porta = dadoToUpdate.Porta,
            Cep = dadoToUpdate.Cep,
            Uf = dadoToUpdate.Uf,
            Cidade = dadoToUpdate.Cidade,
            Endereco = dadoToUpdate.Endereco,
            Bairro = dadoToUpdate.Bairro,
            Complemento = dadoToUpdate.Complemento,
            DataInclusao = dadoToUpdate.DataInclusao,
            DataAlteracao = dadoToUpdate.DataAlteracao
        };

        return Result<EmpresaDto>.Success(dto, "Empresa criado com sucesso.");
    }
}