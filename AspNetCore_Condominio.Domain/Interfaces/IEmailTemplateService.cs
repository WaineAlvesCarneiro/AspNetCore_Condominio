namespace AspNetCore_Condominio.Domain.Interfaces;

public interface IEmailTemplateService
{
    string GerarBoasVindasUsuario(string nomeUsuario, string senhaTemporaria);
    string GerarUsuarioAlterado(string nomeUsuario);

    string GerarBoasVindasEmpresa(string razaoSocial);
    string GerarEmpresaAlterada(string razaoSocial);

    string GerarBoasVindasMorador(string nome);
    string GerarMoradorAlterado(string nome);

    string GerarRedefinicaoSenha(string nomeUsuario, string link);
}