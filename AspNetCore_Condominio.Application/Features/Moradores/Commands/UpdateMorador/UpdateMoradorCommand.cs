using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.ValidatorBase;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.UpdateMorador;

public class UpdateMoradorCommand : IRequest<Result<MoradorDto>>, ICommandMoradorBase
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Celular { get; set; }
    public required string Email { get; set; }
    public bool IsProprietario { get; set; }
    public DateOnly DataEntrada { get; set; }
    public DateOnly DataInclusao { get; set; } = new DateOnly();
    public DateOnly? DataSaida { get; set; }
    public DateOnly? DataAlteracao { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public int ImovelId { get; set; }
}