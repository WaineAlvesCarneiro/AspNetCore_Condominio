namespace AspNetCore_Condominio.Domain.Interfaces;

public interface IEmailSenderService
{
    Task<bool> EnviarSmtpAsync(string para, string assunto, string corpo, long empresaId);
}