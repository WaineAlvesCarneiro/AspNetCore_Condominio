using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetById;

public record GetByIdQueryHandlerImovel(IImovelRepository repository)
    : IRequestHandler<GetByIdQueryImovel, Result<ImovelDto>>
{
    public async Task<Result<ImovelDto>> Handle(GetByIdQueryImovel request, CancellationToken cancellationToken)
    {
        var dado = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (dado is null)
            return Result<ImovelDto>.Failure("Imóvel não encontrado.");

        return Result<ImovelDto>.Success(dado.ToDto());
    }
}