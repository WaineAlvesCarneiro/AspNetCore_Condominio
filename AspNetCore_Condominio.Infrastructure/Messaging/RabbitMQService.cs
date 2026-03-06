using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Infrastructure.Messaging.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AspNetCore_Condominio.Infrastructure.Messaging;

public class RabbitMQService : IMensageriaService, IDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMQService> _logger;

    private IConnection? _connection;
    private IChannel? _channel;

    // Semaphore para garantir que duas threads não tentem criar a conexão ao mesmo tempo
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public RabbitMQService(
        IOptions<RabbitMqSettings> settings,
        ILogger<RabbitMQService> logger)
    {
        _logger = logger;
        _settings = settings.Value;

        _factory = new ConnectionFactory()
        {
            HostName = _settings.Host,
            UserName = _settings.UserName,
            Password = _settings.Password,
            AutomaticRecoveryEnabled = true // Auto-reconecta se o broker cair
        };
    }

    private async Task<IChannel> GetChannelAsync()
    {
        if (_channel is { IsOpen: true })
            return _channel;

        await _connectionLock.WaitAsync();
        try
        {
            // Verificação dupla após o lock (Double-check locking)
            if (_channel is { IsOpen: true })
                return _channel;

            _logger.LogInformation("Estabelecendo nova conexão com RabbitMQ em {Host}...", _settings.Host);

            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            var queueArgs = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", _settings.DlxExchange },
                { "x-dead-letter-routing-key", _settings.QueueName }
            };

            // Configuração Enterprise: Durable=true para persistência em disco
            await _channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs);

            return _channel;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task PublicarMensagemAsync<T>(T mensagem) where T : class
    {
        try
        {
            var channel = await GetChannelAsync();

            var json = JsonSerializer.Serialize(mensagem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            var body = Encoding.UTF8.GetBytes(json);

            // Mensagem persistente: não se perde se o RabbitMQ reiniciar
            var properties = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: _settings.QueueName,
                mandatory: true,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("[RabbitMQ] Mensagem {Tipo} enviada para {Fila}", typeof(T).Name, _settings.QueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[RabbitMQ] Falha crítica ao publicar mensagem do tipo {Tipo}", typeof(T).Name);
            throw; // Importante para o MediatR saber que houve falha na infra
        }
    }

    public void Dispose()
    {
        // Uso de Fire-and-forget ou execução síncrona no Dispose (padrão .NET)
        _channel?.CloseAsync().GetAwaiter().GetResult();
        _connection?.CloseAsync().GetAwaiter().GetResult();
        _channel?.Dispose();
        _connection?.Dispose();
        _connectionLock.Dispose();
        GC.SuppressFinalize(this);
    }
}