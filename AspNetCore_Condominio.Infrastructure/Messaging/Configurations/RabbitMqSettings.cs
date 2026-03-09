namespace AspNetCore_Condominio.Infrastructure.Messaging.Configurations;

public class RabbitMqSettings
{
    public string Host { get; set; } = "localhost";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string QueueName { get; set; } = "fila_emails";
    public string DlxExchange { get; set; } = "dlx_exchange";
    public string DlxQueue { get; set; } = "fila_emails_erro";
    public int Port { get; set; } = 5672;
}