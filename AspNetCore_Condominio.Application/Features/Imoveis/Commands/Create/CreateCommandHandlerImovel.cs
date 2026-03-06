using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.Create;

public class CreateCommandHandlerImovel(
    IImovelRepository repository)
        : IRequestHandler<CreateCommandImovel, Result<ImovelDto>>
{
    public async Task<Result<ImovelDto>> Handle(CreateCommandImovel request, CancellationToken cancellationToken)
    {
        Imovel dado = request.ToEntity();
        await repository.CreateAsync(dado, cancellationToken);

        return Result<ImovelDto>.Success(dado.ToDto(), "Imóvel criado com sucesso.");
    }
}