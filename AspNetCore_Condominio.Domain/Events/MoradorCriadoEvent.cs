using AspNetCore_Condominio.Domain.Entities;
using MediatR;

namespace AspNetCore_Condominio.Domain.Events;

public record MoradorCriadoEvent(Morador Morador, bool IsCreate) : INotification;