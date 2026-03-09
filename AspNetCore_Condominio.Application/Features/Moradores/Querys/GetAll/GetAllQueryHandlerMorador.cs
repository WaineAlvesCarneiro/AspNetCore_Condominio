using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAll;

public class GetAllQueryHandlerMorador(IMoradorRepository repository)
    : IRequestHandler<GetAllQueryMorador, Result<IEnumerable<MoradorDto>>>
{
    public async Task<Result<IEnumerable<MoradorDto>>> Handle(GetAllQueryMorador request, CancellationToken cancellationToken)
    {
        var dados = await repository.GetAllAsync(
            empresaId: request.IdEmpresa, cancellationToken);

        var dtos = dados.Select(dado => dado.ToDto()).ToList();

        return Result<IEnumerable<MoradorDto>>.Success(dtos);
    }
}