using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetImovelById;

public class GetImovelByIdQueryHandler(IImovelRepository imovelRepository)
    : IRequestHandler<GetImovelByIdQuery, Result<ImovelDto>>
{
    public async Task<Result<ImovelDto>> Handle(GetImovelByIdQuery request, CancellationToken cancellationToken)
    {
        var imovel = await imovelRepository.GetByIdAsync(request.Id);
        if (imovel is null)
            return Result<ImovelDto>.Failure("Imóvel não encontrado.");

        var dto = new ImovelDto
        {
            Id = imovel.Id,
            Bloco = imovel.Bloco,
            Apartamento = imovel.Apartamento,
            BoxGaragem = imovel.BoxGaragem
        };

        return Result<ImovelDto>.Success(dto);
    }
}