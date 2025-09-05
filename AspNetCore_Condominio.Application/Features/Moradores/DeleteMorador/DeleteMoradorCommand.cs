using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.DeleteMorador;

public record DeleteMoradorCommand(int Id) : IRequest<Result>;