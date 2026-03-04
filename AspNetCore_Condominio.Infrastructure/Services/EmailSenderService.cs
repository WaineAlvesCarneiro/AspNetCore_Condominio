using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace AspNetCore_Condominio.Infrastructure.Services;

public class EmailSenderService(IEmpresaRepository empresaRepository, ILogger<EmailSenderService> logger) : IEmailSenderService
{
    private readonly IEmpresaRepository _empresaRepository = empresaRepository;
    private readonly ILogger<EmailSenderService> _logger = logger;

    public async Task<bool> EnviarSmtpAsync(string para, string assunto, string corpo, long empresaId)
    {
        var empresa = await _empresaRepository.GetByIdAsync(empresaId);

        if (empresa == null || string.IsNullOrEmpty(empresa.Senha))
        {
            _logger.LogError("Configurações de e-mail não encontradas para empresa {Id}", empresaId);
            return false;
        }

        try
        {
            using SmtpClient smtp = await SmtpCliente(para, assunto, corpo, empresa);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao disparar e-mail via SMTP para {Destino}", para);
            return false;
        }
    }

    private static async Task<SmtpClient> SmtpCliente(string para, string assunto, string corpo, Domain.Entities.Empresa empresa)
    {
        string senhaReal = EncryptionHelper.Decrypt(empresa.Senha!);

        MimeMessage email;
        SmtpClient smtp;
        CriarEmail(para, assunto, corpo, empresa, out email, out smtp);

        await smtp.ConnectAsync(empresa.Host, empresa.Porta, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(empresa.Email, senhaReal);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        return smtp;
    }

    private static void CriarEmail(string para, string assunto, string corpo, Domain.Entities.Empresa empresa, out MimeMessage email, out SmtpClient smtp)
    {
        email = new MimeMessage();
        email.From.Add(new MailboxAddress(empresa.Fantasia, empresa.Email));
        email.To.Add(new MailboxAddress("", para));
        email.Subject = assunto;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = corpo };
        smtp = new SmtpClient();
        smtp.Timeout = 10000;
    }
}