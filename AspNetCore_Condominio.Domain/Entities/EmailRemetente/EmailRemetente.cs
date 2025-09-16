namespace AspNetCore_Condominio.Domain.Entities.EmailRemetente;

public class EmailRemetente
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Senha { get; set; }
    public required string Host { get; set; }
    public int Port { get; set; }
}
