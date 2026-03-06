namespace AspNetCore_Condominio.Domain.Interfaces;

public interface IMensageriaService
{
    Task PublicarMensagemAsync<T>(T mensagem) where T : class;
}

public record EnvioEmailRequest(string Para, string Assunto, string Corpo, long EmpresaId);