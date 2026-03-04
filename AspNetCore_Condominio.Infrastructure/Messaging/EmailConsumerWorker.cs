using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AspNetCore_Condominio.Infrastructure.Messaging;

public class EmailConsumerWorker(ILogger<EmailConsumerWorker> logger, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ILogger<EmailConsumerWorker> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly string _hostname = "localhost";
    private readonly string _queueName = "fila_emails";

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory() { HostName = _hostname };
        using var connection = await factory.CreateConnectionAsync(cancellationToken);
        using var _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(0, 10, false, cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        MontaEEnviarSmtp(_channel, consumer);

        await _channel.BasicConsumeAsync(_queueName, false, consumer, cancellationToken: cancellationToken);
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    private void MontaEEnviarSmtp(IChannel _channel, AsyncEventingBasicConsumer consumer)
    {
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var emailData = JsonSerializer.Deserialize<MensagemEmailDto>(
                Encoding.UTF8.GetString(ea.Body.ToArray()));

            using var scope = _serviceProvider.CreateScope();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();

            // A mágica acontece aqui:
            var sucesso = await emailSender.EnviarSmtpAsync(
                emailData!.Para,
                emailData.Assunto,
                emailData.Corpo,
                emailData.EmpresaId);

            if (sucesso)
            {
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
                _logger.LogInformation("Fila processada com sucesso.");
            }
            else
            {
                // Se falhar, re-enfileira para tentar de novo
                await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
            }
        };
    }
}