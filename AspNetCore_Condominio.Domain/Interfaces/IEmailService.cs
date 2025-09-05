namespace AspNetCore_Condominio.Domain.Interfaces;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}