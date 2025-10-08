using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Queries.GetMoradorById;

public class GetMoradorByIdQueryHandler(IMoradorRepository moradorRepository)
    : IRequestHandler<GetMoradorByIdQuery, Result<MoradorDto>>
{
    public async Task<Result<MoradorDto>> Handle(GetMoradorByIdQuery request, CancellationToken cancellationToken)
    {
        var morador = await moradorRepository.GetByIdAsync(request.Id);
        if (morador is null)
            return Result<MoradorDto>.Failure("Morador não encontrado.");

        var dto = new MoradorDto
        {
            Id = morador.Id,
            Nome = morador.Nome,
            Celular = morador.Celular,
            Email = morador.Email,
            IsProprietario = morador.IsProprietario,
            DataEntrada = DateOnly.FromDateTime(morador.DataEntrada),
            DataSaida = morador.DataSaida.HasValue ? DateOnly.FromDateTime(morador.DataSaida.Value) : null,
            DataInclusao = DateOnly.FromDateTime(morador.DataInclusao),
            DataAlteracao = morador.DataAlteracao.HasValue ? DateOnly.FromDateTime(morador.DataAlteracao.Value) : null,
            ImovelId = morador.ImovelId,
            ImovelDto = morador.Imovel != null
                ? new ImovelDto
                {
                    Id = morador.Imovel.Id,
                    Bloco = morador.Imovel.Bloco,
                    Apartamento = morador.Imovel.Apartamento,
                    BoxGaragem = morador.Imovel.BoxGaragem
                }
                : null
        };

        return Result<MoradorDto>.Success(dto);
    }
}
