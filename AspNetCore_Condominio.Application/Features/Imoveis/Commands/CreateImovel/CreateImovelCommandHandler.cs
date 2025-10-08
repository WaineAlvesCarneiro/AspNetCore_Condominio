using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.CreateImovel;

public class CreateImovelCommandHandler(IImovelRepository imovelRepository)
    : IRequestHandler<CreateImovelCommand, Result<ImovelDto>>
{
    public async Task<Result<ImovelDto>> Handle(CreateImovelCommand request, CancellationToken cancellationToken)
    {
        var imovel = new Imovel
        {
            Bloco = request.Bloco,
            Apartamento = request.Apartamento,
            BoxGaragem = request.BoxGaragem
        };

        await imovelRepository.CreateAsync(imovel);

        var dto = new ImovelDto
        {
            Id = imovel.Id,
            Bloco = imovel.Bloco,
            Apartamento = imovel.Apartamento,
            BoxGaragem = imovel.BoxGaragem
        };

        return Result<ImovelDto>.Success(dto, "Imóvel criado com sucesso.");
    }
}