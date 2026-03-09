using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAll;

public record GetAllQueryHandlerImovel(IImovelRepository repository)
    : IRequestHandler<GetAllQueryImovel, Result<IEnumerable<ImovelDto>>>
{
    public async Task<Result<IEnumerable<ImovelDto>>> Handle(GetAllQueryImovel request, CancellationToken cancellationToken)
    {
        var dados = await repository.GetAllAsync(
            empresaId: request.IdEmpresa, cancellationToken);

        var dtos = dados.Select(dado => dado.ToDto()).ToList();

        return Result<IEnumerable<ImovelDto>>.Success(dtos);
    }
}
