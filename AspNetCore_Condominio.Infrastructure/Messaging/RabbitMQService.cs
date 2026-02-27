using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AspNetCore_Condominio.Infrastructure.Messaging;

public class RabbitMQService : IMensageriaService
{
    private readonly string _hostname = "localhost";

    public async Task EnviarEmailAsync(string para, string assunto, string corpoHtml, long? empresaId)
    {
        // 1. A Factory agora prefere o CreateConnectionAsync
        var factory = new ConnectionFactory() { HostName = _hostname };

        // 2. Use 'await' e as versões Async dos métodos
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // 3. Declaração da fila (também assíncrona)
        await channel.QueueDeclareAsync(
            queue: "fila_emails",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var mensagem = new MensagemEmailDto
        {
            EmpresaId = empresaId ?? 0,
            Para = para,
            Assunto = assunto,
            Corpo = corpoHtml
        };

        var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(mensagem));

        // 4. Publicação da mensagem
        // No RabbitMQ 7+, usamos BasicPublishAsync
        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "fila_emails",
            body: messageBody);

        Console.WriteLine($"[x] Mensagem enviada para a fila: {para}");
    }

    public Task EnviarWhatsappAsync(string numero, string mensagem)
    {
        // Implementaremos depois seguindo a mesma lógica
        return Task.CompletedTask;
    }
}