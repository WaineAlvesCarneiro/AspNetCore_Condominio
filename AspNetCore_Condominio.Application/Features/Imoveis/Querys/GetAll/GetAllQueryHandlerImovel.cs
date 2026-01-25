using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAll;

public record GetAllQueryHandlerImovel(IImovelRepository repository)
    : IRequestHandler<GetAllQueryImovel, Result<IEnumerable<ImovelDto>>>
{
    public async Task<Result<IEnumerable<ImovelDto>>> Handle(GetAllQueryImovel request, CancellationToken cancellationToken)
    {
        var dados = await repository.GetAllAsync();

        var dtos = dados.Select(dado => new ImovelDto
        {
            Id = dado.Id,
            Bloco = dado.Bloco,
            Apartamento = dado.Apartamento,
            BoxGaragem = dado.BoxGaragem,
            EmpresaId = dado.EmpresaId
        });

        return Result<IEnumerable<ImovelDto>>.Success(dtos);
    }
}
