using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.Update;

public class UpdateCommandHandlerImovel(IImovelRepository repository)
    : IRequestHandler<UpdateCommandImovel, Result<ImovelDto>>
{
    private readonly IImovelRepository _repository = repository;

    public async Task<Result<ImovelDto>> Handle(UpdateCommandImovel request, CancellationToken cancellationToken)
    {
        var dadoToUpdate = await _repository.GetByIdAsync(request.Id);
        if (dadoToUpdate == null)
        {
            return Result<ImovelDto>.Failure("Imóvel não encontrado.");
        }

        dadoToUpdate.Bloco = request.Bloco;
        dadoToUpdate.Apartamento = request.Apartamento;
        dadoToUpdate.BoxGaragem = request.BoxGaragem;
        dadoToUpdate.EmpresaId = request.EmpresaId;

        await _repository.UpdateAsync(dadoToUpdate);

        var dto = new ImovelDto
        {
            Id = dadoToUpdate.Id,
            Bloco = dadoToUpdate.Bloco,
            Apartamento = dadoToUpdate.Apartamento,
            BoxGaragem = dadoToUpdate.BoxGaragem,
            EmpresaId = dadoToUpdate.EmpresaId
        };

        return Result<ImovelDto>.Success(dto, "Imóvel criado com sucesso.");
    }
}