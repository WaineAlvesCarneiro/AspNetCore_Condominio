using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllPaged;

public class GetAllPagedQueryHandlerMorador(IMoradorRepository repository)
    : IRequestHandler<GetAllPagedQueryMorador, Result<PagedResult<MoradorDto>>>
{
    private readonly IMoradorRepository _repository = repository;

    public async Task<Result<PagedResult<MoradorDto>>> Handle(
        GetAllPagedQueryMorador request,
        CancellationToken cancellationToken)
    {
        (IEnumerable<Morador> items, int totalCount) = await _repository.GetAllPagedAsync(
            userEmpresaId: request.UserEmpresaId,
            page: request.ActualPage,
            pageSize: request.ActualPageSize,
            orderBy: request.ActualSortBy,
            direction: request.ActualDirection,
            searchTerm: request.SearchTerm
        );

        var dtos = items.Select(dado => new MoradorDto
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
            ImovelDto = dado.Imovel != null ? new ImovelDto
            {
                Id = dado.Imovel.Id,
                Bloco = dado.Imovel.Bloco,
                Apartamento = dado.Imovel.Apartamento,
                BoxGaragem = dado.Imovel.BoxGaragem,
                EmpresaId = dado.Imovel.EmpresaId,
            } : null,
            EmpresaId = dado.EmpresaId,
            EmpresaDto = dado.Empresa != null ? new EmpresaDto
            {
                Id = dado.Empresa.Id,
                RazaoSocial = dado.Empresa.RazaoSocial,
                Fantasia = dado.Empresa.Fantasia,
                Cnpj = dado.Empresa.Cnpj,
                TipoDeCondominio = dado.Empresa.TipoDeCondominio,
                Nome = dado.Empresa.Nome,
                Celular = dado.Empresa.Celular,
                Telefone = dado.Empresa.Telefone!,
                Email = dado.Empresa.Email,
                Senha = dado.Empresa.Senha,
                Host = dado.Empresa.Host,
                Porta = dado.Empresa.Porta,
                Cep = dado.Empresa.Cep,
                Uf = dado.Empresa.Uf,
                Cidade = dado.Empresa.Cidade,
                Endereco = dado.Empresa.Endereco,
                Complemento = dado.Empresa.Complemento,
                DataInclusao = dado.Empresa.DataInclusao,
                DataAlteracao = dado.Empresa.DataAlteracao
            } : null
        });

        return Result<PagedResult<MoradorDto>>.Success(new PagedResult<MoradorDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = request.ActualPage,
            LinesPerPage = request.ActualPageSize
        });
    }
}