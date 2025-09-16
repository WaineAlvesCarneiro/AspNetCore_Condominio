using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllPagedMoradores;

public class GetAllPagedMoradoresQueryHandler(IMoradorRepository moradorRepository)
    : IRequestHandler<GetAllPagedMoradoresQuery, Result<PagedResult<MoradorDto>>>
{
    private readonly IMoradorRepository _moradorRepository = moradorRepository;

    public async Task<Result<PagedResult<MoradorDto>>> Handle(GetAllPagedMoradoresQuery request, CancellationToken cancellationToken)
    {
        (IEnumerable<Morador> items, int totalCount) = await _moradorRepository.GetAllPagedAsync(
            request.Page,
            request.LinesPerPage,
            request.OrderBy,
            request.Direction
        );

        IEnumerable<MoradorDto> dtos = items.Select(m => new MoradorDto
        {
            Id = m.Id,
            Nome = m.Nome,
            Celular = m.Celular,
            Email = m.Email,
            IsProprietario = m.IsProprietario,
            DataEntrada = DateOnly.FromDateTime(m.DataEntrada),
            DataSaida = m.DataSaida.HasValue ? DateOnly.FromDateTime(m.DataSaida.Value) : null,
            DataInclusao = DateOnly.FromDateTime(m.DataInclusao),
            DataAlteracao = m.DataAlteracao.HasValue ? DateOnly.FromDateTime(m.DataAlteracao.Value) : null,
            ImovelId = m.ImovelId,
            ImovelDto = m.Imovel != null ? new ImovelDto
            {
                Id = m.Imovel.Id,
                Bloco = m.Imovel.Bloco,
                Apartamento = m.Imovel.Apartamento,
                BoxGaragem = m.Imovel.BoxGaragem
            } : null
        });

        PagedResult<MoradorDto> pagedResult = new PagedResult<MoradorDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = request.Page,
            LinesPerPage = request.LinesPerPage
        };

        return Result<PagedResult<MoradorDto>>.Success(pagedResult);
    }
}