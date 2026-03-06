using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Infrastructure.Messaging.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AspNetCore_Condominio.Infrastructure.Messaging;

public class EmailConsumerWorker : BackgroundService
{
    private readonly ILogger<EmailConsumerWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqSettings _settings;
    private readonly ConnectionFactory _factory;

    public EmailConsumerWorker(
        ILogger<EmailConsumerWorker> logger,
        IServiceProvider serviceProvider,
        IOptions<RabbitMqSettings> settings)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _settings = settings.Value;

        _factory = new ConnectionFactory()
        {
            HostName = _settings.Host,
            UserName = _settings.UserName,
            Password = _settings.Password,
            Port = _settings.Port,
            AutomaticRecoveryEnabled = true
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Worker] Aguardando inicialização do barramento...");

        // Loop de resiliência: Se o RabbitMQ estiver fora, o Worker espera, 
        // mas a API continua funcionando normalmente.
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var connection = await _factory.CreateConnectionAsync(stoppingToken);
                await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

                _logger.LogInformation("[Worker] Conectado! Configurando infraestrutura...");

                // --- SETUP DE INFRAESTRUTURA ---
                await channel.ExchangeDeclareAsync(_settings.DlxExchange, ExchangeType.Direct);
                await channel.QueueDeclareAsync(_settings.DlxQueue, true, false, false);
                await channel.QueueBindAsync(_settings.DlxQueue, _settings.DlxExchange, _settings.QueueName);

                var queueArgs = new Dictionary<string, object?>
                {
                    { "x-dead-letter-exchange", _settings.DlxExchange },
                    { "x-dead-letter-routing-key", _settings.QueueName }
                };
                await channel.QueueDeclareAsync(_settings.QueueName, true, false, false, queueArgs);
                await channel.BasicQosAsync(0, 10, false, stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try { await ProcessarMensagemAsync(channel, ea); }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[Worker] Erro crítico na mensagem {ID}", ea.DeliveryTag);
                        await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                    }
                };

                await channel.BasicConsumeAsync(_settings.QueueName, false, consumer, stoppingToken);

                // Mantém o loop vivo enquanto a conexão estiver aberta
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Worker] RabbitMQ offline ou inacessível. Tentando novamente em 10s... Erro: {Msg}", ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Espera antes de tentar conectar de novo
            }
        }
    }

    private async Task ProcessarMensagemAsync(IChannel channel, BasicDeliverEventArgs ea)
    {
        var content = Encoding.UTF8.GetString(ea.Body.ToArray());

        // Configuração para aceitar propriedades que começam com letra minúscula (camelCase)
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var emailData = JsonSerializer.Deserialize<EnvioEmailRequest>(content, options);

        using var scope = _serviceProvider.CreateScope();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();

        // Agora emailData?.Para não será mais nulo!
        _logger.LogInformation("[Worker] Processando e-mail para: {Destino}", emailData?.Para);

        // Validação de segurança para não tentar enviar se o e-mail for nulo
        if (emailData == null || string.IsNullOrEmpty(emailData.Para))
        {
            _logger.LogWarning("[Worker] Dados de e-mail incompletos. Movendo para DLQ.");
            await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
            return;
        }

        var sucesso = await emailSender.EnviarSmtpAsync(
            emailData.Para, emailData.Assunto, emailData.Corpo, emailData.EmpresaId);

        if (sucesso)
            await channel.BasicAckAsync(ea.DeliveryTag, false);
        else
            await TratarRe_tentativaAsync(channel, ea);
    }

    private async Task TratarRe_tentativaAsync(IChannel channel, BasicDeliverEventArgs ea)
    {
        // Se os dados básicos são nulos, não adianta tentar de novo. Mande para a DLQ direto.
        var content = Encoding.UTF8.GetString(ea.Body.ToArray());
        if (string.IsNullOrEmpty(content) || content.Contains("\"empresaId\":0"))
        {
            _logger.LogError("[Worker] Mensagem inválida detectada. Descartando para DLQ.");
            await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
            return;
        }

        // Para erros reais de rede/SMTP, usamos a contagem de x-death
        long retryCount = 0;
        if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.TryGetValue("x-death", out var xDeath))
        {
            var deathList = xDeath as IList<object>;
            retryCount = deathList?.Count ?? 0;
        }

        if (retryCount < 3)
        {
            _logger.LogWarning("[Worker] Falha no envio. Tentativa {N}. Re-enfileirando via DLX...", retryCount + 1);
            // IMPORTANTE: requeue: false faz a mensagem ir para a DLX. 
            // Se a DLX estiver ligada de volta à fila original, ela volta com delay.
            await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
        }
        else
        {
            _logger.LogError("[Worker] Limite de tentativas atingido. Removendo da fila principal.");
            await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
        }
    }
}