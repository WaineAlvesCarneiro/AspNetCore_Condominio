using AspNetCore_Condominio.Domain.Interfaces;
using System.Collections.Concurrent;

namespace AspNetCore_Condominio.Integration.Tests.Mocks;

public class FakeEmailService : IEmailService
{
    public ConcurrentBag<EmailMessage> SentEmails { get; } = new ConcurrentBag<EmailMessage>();

    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        SentEmails.Add(new EmailMessage(to, subject, body));
        return Task.CompletedTask;
    }
}

public record EmailMessage(string To, string Subject, string Body);