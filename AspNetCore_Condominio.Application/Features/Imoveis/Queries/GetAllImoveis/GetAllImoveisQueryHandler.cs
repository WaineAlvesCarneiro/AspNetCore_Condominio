using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAllImoveis;

public class GetAllImoveisQueryHandler(IImovelRepository imovelRepository)
    : IRequestHandler<GetAllImoveisQuery, Result<IEnumerable<ImovelDto>>>
{
    public async Task<Result<IEnumerable<ImovelDto>>> Handle(GetAllImoveisQuery request, CancellationToken cancellationToken)
    {
        var imoveis = await imovelRepository.GetAllAsync();

        var dtos = imoveis.Select(imovel => new ImovelDto
        {
            Id = imovel.Id,
            Bloco = imovel.Bloco,
            Apartamento = imovel.Apartamento,
            BoxGaragem = imovel.BoxGaragem
        });

        return Result<IEnumerable<ImovelDto>>.Success(dtos);
    }
}
