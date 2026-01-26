using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetById;

public record GetByIdQueryHandlerImovel(IImovelRepository repository)
    : IRequestHandler<GetByIdQueryImovel, Result<ImovelDto>>
{
    public async Task<Result<ImovelDto>> Handle(GetByIdQueryImovel request, CancellationToken cancellationToken)
    {
        var dado = await repository.GetByIdAsync(request.Id, request.UserEmpresaId);
        if (dado is null)
            return Result<ImovelDto>.Failure("Imóvel não encontrado.");

        var dto = new ImovelDto
        {
            Id = dado.Id,
            Bloco = dado.Bloco,
            Apartamento = dado.Apartamento,
            BoxGaragem = dado.BoxGaragem,
            EmpresaId = dado.EmpresaId
        };

        return Result<ImovelDto>.Success(dto);
    }
}