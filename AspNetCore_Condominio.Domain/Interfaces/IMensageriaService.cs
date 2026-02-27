namespace AspNetCore_Condominio.Domain.Interfaces;

public interface IMensageriaService
{
    Task EnviarEmailAsync(string para, string assunto, string corpoHtml, long? empresaId);
    Task EnviarWhatsappAsync(string numero, string mensagem);
}
