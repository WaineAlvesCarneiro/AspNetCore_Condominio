using AspNetCore_Condominio.Domain.Entities.EmailRemetente;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Infrastructure.Repositories.Email;
using System.Net;
using System.Net.Mail;

namespace AspNetCore_Condominio.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailRepository _emailRepository;

    public EmailService(EmailRepository emailRepository)
    {
        _emailRepository = emailRepository;
    }

    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        EmailRemetente emailSettings = await _emailRepository.GetAsync();

        if (emailSettings == null)
        {
            Console.WriteLine("Erro: Configurações de e-mail não encontradas no banco de dados.");
            return;
        }

        SmtpClient smtpClient = new(emailSettings.Host)
        {
            Port = emailSettings.Port,
            Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Senha),
            EnableSsl = true
        };

        MailMessage mailMessage = new()
        {
            From = new MailAddress(emailSettings.Username)
        };
        mailMessage.To.Add(new MailAddress(to));
        mailMessage.Subject = subject;
        mailMessage.IsBodyHtml = false;
        mailMessage.Body = body;

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine($"E-mail enviado com sucesso para: {to}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
        }
    }
}
