using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Helpers; // Importante para o Decrypt
using AspNetCore_Condominio.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection; // Necessário para .CreateScope()
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AspNetCore_Condominio.Infrastructure.Messaging;

public class EmailConsumerWorker : BackgroundService
{
    private readonly ILogger<EmailConsumerWorker> _logger;
    private readonly IServiceProvider _serviceProvider; // Adicionado aqui
    private readonly string _hostname = "localhost";
    private readonly string _queueName = "fila_emails";

    public EmailConsumerWorker(ILogger<EmailConsumerWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider; // Injetado aqui
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = _hostname };
        using var connection = await factory.CreateConnectionAsync(stoppingToken);
        using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        _logger.LogInformation(" [*] Aguardando mensagens na fila: {Queue}", _queueName);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);
            var emailData = JsonSerializer.Deserialize<MensagemEmailDto>(messageJson);

            using (var scope = _serviceProvider.CreateScope())
            {
                var empresaRepository = scope.ServiceProvider.GetRequiredService<IEmpresaRepository>();
                var empresa = await empresaRepository.GetByIdAsync(emailData?.EmpresaId ?? 0);

                if (empresa == null || string.IsNullOrEmpty(empresa.Senha))
                {
                    _logger.LogError("Configurações de e-mail inválidas. Removendo da fila.");
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                    return;
                }

                try
                {
                    string senhaReal = EncryptionHelper.Decrypt(empresa.Senha);

                    using var email = new MimeKit.MimeMessage();
                    email.From.Add(new MimeKit.MailboxAddress(empresa.Fantasia, empresa.Email));
                    email.To.Add(new MimeKit.MailboxAddress("", emailData?.Para));
                    email.Subject = emailData?.Assunto;
                    email.Body = new MimeKit.TextPart(MimeKit.Text.TextFormat.Html) { Text = emailData?.Corpo };

                    using var smtp = new MailKit.Net.Smtp.SmtpClient();

                    // Timeout curto para não travar o worker
                    smtp.Timeout = 10000;

                    await smtp.ConnectAsync(empresa.Host, empresa.Porta, MailKit.Security.SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(empresa.Email, senhaReal);
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);

                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                    _logger.LogInformation("E-mail enviado com sucesso para: {Destinatario}", emailData?.Para);
                }
                catch (MailKit.Net.Smtp.SmtpCommandException ex)
                {
                    _logger.LogError("Erro de comando SMTP (provavelmente autenticação/senha): {Msg}", ex.Message);
                    // Se a senha estiver errada, não adianta tentar de novo agora. 
                    // Aguarda 30 segundos antes de devolver para a fila para evitar o ban do IP
                    await Task.Delay(30000);
                    await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha geral no Worker. Aguardando 10s antes de re-tentar.");
                    await Task.Delay(10000);
                    await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
                }
            }
        };

        await channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}