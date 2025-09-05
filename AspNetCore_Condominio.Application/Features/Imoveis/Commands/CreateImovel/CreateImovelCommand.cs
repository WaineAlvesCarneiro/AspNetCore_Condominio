using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Imoveis.Commands.ValidatorBase;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.CreateImovel;

public class CreateImovelCommand : IRequest<Result<ImovelDto>>, ICommandImovelBase
{
    public int Id { get; set; }
    public required string Bloco { get; set; }
    public required string Apartamento { get; set; }
    public required string BoxGaragem { get; set; }
}