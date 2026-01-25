namespace AspNetCore_Condominio.Domain.Interfaces;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken);
}

public interface IEntidadeComEmail
{
    string? Nome { get; }
    string Email { get; }
}