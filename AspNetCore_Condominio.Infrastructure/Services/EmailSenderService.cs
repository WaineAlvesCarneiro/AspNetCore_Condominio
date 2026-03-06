using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace AspNetCore_Condominio.Infrastructure.Services;

public class EmailSenderService(
    IEmpresaRepository empresaRepository,
    ILogger<EmailSenderService> logger,
    IMemoryCache cache)
        : IEmailSenderService
{
    public async Task<bool> EnviarSmtpAsync(string para, string assunto, string corpo, long empresaId)
    {
        // 1. Estratégia de Cache para evitar idas excessivas ao Banco de Dados
        var empresa = await cache.GetOrCreateAsync($"empresa_smtp_{empresaId}", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10); // Cache por 10 min
            logger.LogInformation("Buscando configurações de SMTP no banco para empresa {Id}", empresaId);
            return empresaRepository.GetByIdAsync(empresaId);
        });

        if (empresa == null || string.IsNullOrEmpty(empresa.Senha))
        {
            logger.LogError("[SMTP] Configurações não encontradas para empresa {Id}", empresaId);
            return false;
        }

        try
        {
            var email = MontarMensagem(para, assunto, corpo, empresa);
            await DispararEmailAsync(email, empresa);

            logger.LogInformation("[SMTP] E-mail enviado com sucesso para {Destino}", para);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[SMTP] Falha ao disparar e-mail para {Destino} via host {Host}", para, empresa.Host);
            return false;
        }
    }

    private static MimeMessage MontarMensagem(string para, string assunto, string corpo, Domain.Entities.Empresa empresa)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(empresa.Fantasia, empresa.Email));
        email.To.Add(new MailboxAddress("", para));
        email.Subject = assunto;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = corpo };
        return email;
    }

    private static async Task DispararEmailAsync(MimeMessage email, Domain.Entities.Empresa empresa)
    {
        using var smtp = new SmtpClient();
        smtp.Timeout = 15000; // 15 segundos de timeout

        // Descriptografia da senha (conforme seu Helper existente)
        string senhaReal = EncryptionHelper.Decrypt(empresa.Senha!);

        // Conexão e Autenticação
        await smtp.ConnectAsync(empresa.Host, empresa.Porta, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(empresa.Email, senhaReal);

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}