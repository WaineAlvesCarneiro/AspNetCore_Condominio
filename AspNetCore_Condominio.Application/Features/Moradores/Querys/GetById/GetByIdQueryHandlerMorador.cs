using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Queries.GetById;

public class GetByIdQueryHandlerMorador(IMoradorRepository repository)
    : IRequestHandler<GetByIdQueryMorador, Result<MoradorDto>>
{
    public async Task<Result<MoradorDto>> Handle(GetByIdQueryMorador request, CancellationToken cancellationToken)
    {
        var dado = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (dado is null)
            return Result<MoradorDto>.Failure("Morador não encontrado.");

        return Result<MoradorDto>.Success(dado.ToDto());
    }
}
