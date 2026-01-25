using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.Create;

public class CreateCommandHandlerImovel(IImovelRepository repository)
    : IRequestHandler<CreateCommandImovel, Result<ImovelDto>>
{
    public async Task<Result<ImovelDto>> Handle(CreateCommandImovel request, CancellationToken cancellationToken)
    {
        var dado = new Imovel
        {
            Bloco = request.Bloco,
            Apartamento = request.Apartamento,
            BoxGaragem = request.BoxGaragem
        };

        await repository.CreateAsync(dado);

        var dto = new ImovelDto
        {
            Id = dado.Id,
            Bloco = dado.Bloco,
            Apartamento = dado.Apartamento,
            BoxGaragem = dado.BoxGaragem
        };

        return Result<ImovelDto>.Success(dto, "Imóvel criado com sucesso.");
    }
}