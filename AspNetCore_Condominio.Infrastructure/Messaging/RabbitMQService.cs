using AspNetCore_Condominio.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AspNetCore_Condominio.Infrastructure.Messaging;

public class RabbitMQService : IMensageriaService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private const string QueueName = "fila_emails";

    public RabbitMQService(IConfiguration configuration)
    {
        var factory = new ConnectionFactory()
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = configuration["RabbitMQ:UserName"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest"
        };

        // Conecta uma única vez no início
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        _channel.QueueDeclareAsync(QueueName, true, false, false).GetAwaiter().GetResult();
    }

    public async Task PublicarEmailFilaAsync(EnvioEmailRequest request)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
        await _channel.BasicPublishAsync(string.Empty, QueueName, body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}