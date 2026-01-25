namespace AspNetCore_Condominio.Domain.Entities.EmailRemetente;

public class EmailRemetente
{
    public long Id { get; set; }
    public long EmpresaId { get; set; }
    public required string Username { get; set; }
    public required string Senha { get; set; }
    public required string Host { get; set; }
    public int Port { get; set; }

    public EmailRemetente(string username, string senha, string host, int port)
    {
        if (port <= 0)
        {
            throw new ArgumentException("A porta deve ser um número positivo.", nameof(port));
        }

        Username = username;
        Senha = senha;
        Host = host;
        Port = port;
    }

    public EmailRemetente() { }
}
