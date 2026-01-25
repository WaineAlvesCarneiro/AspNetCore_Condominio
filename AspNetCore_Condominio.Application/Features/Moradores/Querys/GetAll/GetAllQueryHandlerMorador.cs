using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAll;

public class GetAllQueryHandlerMorador(IMoradorRepository repository)
    : IRequestHandler<GetAllQueryMorador, Result<IEnumerable<MoradorDto>>>
{
    public async Task<Result<IEnumerable<MoradorDto>>> Handle(GetAllQueryMorador request, CancellationToken cancellationToken)
    {
        var dados = await repository.GetAllAsync();

        var dtos = dados.Select(dado => new MoradorDto
        {
            Id = dado.Id,
            Nome = dado.Nome,
            Celular = dado.Celular,
            Email = dado.Email,
            IsProprietario = dado.IsProprietario,
            DataEntrada = dado.DataEntrada,
            DataSaida = dado.DataSaida,
            DataInclusao = dado.DataInclusao,
            DataAlteracao = dado.DataAlteracao,
            ImovelId = dado.ImovelId,
            ImovelDto = dado.Imovel != null
                ? new ImovelDto
                {
                    Id = dado.Imovel.Id,
                    Bloco = dado.Imovel.Bloco,
                    Apartamento = dado.Imovel.Apartamento,
                    BoxGaragem = dado.Imovel.BoxGaragem,
                    EmpresaId = dado.Imovel.EmpresaId
                }
                : null,
            EmpresaId = dado.EmpresaId,
            EmpresaDto = dado.Empresa != null
                ? new EmpresaDto
                {
                    Id = dado.Id,
                    RazaoSocial = dado.Empresa.RazaoSocial,
                    Fantasia = dado.Empresa.Fantasia,
                    Cnpj = dado.Empresa.Cnpj,
                    TipoDeCondominio = dado.Empresa.TipoDeCondominio,
                    Nome = dado.Nome,
                    Celular = dado.Empresa.Celular,
                    Telefone = dado.Empresa.Telefone!,
                    Email = dado.Empresa.Email,
                    Cep = dado.Empresa.Cep,
                    Uf = dado.Empresa.Uf,
                    Cidade = dado.Empresa.Cidade,
                    Endereco = dado.Empresa.Endereco,
                    Complemento = dado.Empresa.Complemento,
                    DataInclusao = dado.Empresa.DataInclusao,
                    DataAlteracao = dado.Empresa.DataAlteracao
                }
                : null
        });

        return Result<IEnumerable<MoradorDto>>.Success(dtos);
    }
}