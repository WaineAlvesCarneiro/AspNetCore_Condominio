using AspNetCore_Condominio.Domain.Events;
using AspNetCore_Condominio.Domain.Interfaces;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Events;

public class MoradorCriadoEventHandler(IEmailService emailService) : INotificationHandler<MoradorCriadoEvent>
{
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(MoradorCriadoEvent notification, CancellationToken cancellationToken)
    {
        string assunto;
        string corpo;

        if (notification.IsCreate)
        {
            assunto = "Bem-vindo ao Condomínio! Asp Net Core";
            corpo = $"Olá, **{notification.Morador.Nome}**! Seu cadastro foi criado com sucesso.";
        }
        else
        {
            assunto = "Atualização de Cadastro no Condomínio! Asp Net Core";
            corpo = $"Olá, **{notification.Morador.Nome}**! Seu cadastro foi atualizado com sucesso.";
        }

        await _emailService.SendAsync(notification.Morador.Email, assunto, corpo);
    }
}
