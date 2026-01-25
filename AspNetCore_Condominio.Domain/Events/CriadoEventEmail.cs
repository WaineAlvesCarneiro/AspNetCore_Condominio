using MediatR;

namespace AspNetCore_Condominio.Domain.Events;

public record CriadoEventEmail<TEntity>(TEntity Entidade, bool IsCreate) : INotification
    where TEntity : class;