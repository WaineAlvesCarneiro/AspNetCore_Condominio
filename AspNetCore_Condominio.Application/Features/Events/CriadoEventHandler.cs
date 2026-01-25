using AspNetCore_Condominio.Domain.Events;
using AspNetCore_Condominio.Domain.Interfaces;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Events;

public class CriadoEventHandler<TEntity>(IEmailService emailService)
    : INotificationHandler<CriadoEventEmail<TEntity>>
    where TEntity : class, IEntidadeComEmail
{
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(CriadoEventEmail<TEntity> notification, CancellationToken cancellationToken)
    {
        var destinatarioNome = notification.Entidade.Nome;
        var destinatarioEmail = notification.Entidade.Email;

        string assunto = notification.IsCreate ? "Bem-vindo!" : "Atualização de Cadastro!";

        string tipoAcao = notification.IsCreate ? "criado" : "atualizado";
        string corpo = $"Olá, **{destinatarioNome}**! Seu cadastro foi {tipoAcao} com sucesso.";

        await _emailService.SendAsync(destinatarioEmail, assunto, corpo, cancellationToken);
    }
}