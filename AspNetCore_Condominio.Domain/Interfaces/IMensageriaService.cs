namespace AspNetCore_Condominio.Domain.Interfaces;

public interface IMensageriaService
{
    Task PublicarEmailFilaAsync(EnvioEmailRequest email);
}

public record EnvioEmailRequest(string Para, string Assunto, string Corpo, long EmpresaId);