using AspNetCore_Condominio.Domain.Interfaces;
using System.Text;

namespace AspNetCore_Condominio.Infrastructure.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private string MontarLayoutBase(string titulo, string saudacao, string mensagem, string? conteudoExtra = null)
    {
        return $@"
        <div style='font-family: sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px;'>
            <h2 style='color: #2c3e50;'>{titulo}</h2>
            <p>Olá <strong>{saudacao}</strong>,</p>
            <p>{mensagem}</p>
            {conteudoExtra}
            <p style='color: #7f8c8d; font-size: 0.9em;'>Por segurança, você deverá alterar esta senha no seu primeiro acesso.</p>
            <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
            <p style='font-size: 0.8em; color: #bdc3c7;'>Este é um e-mail automático, por favor não responda.</p>
        </div>";
    }

    private string GerarQuadroInformativo(params (string Label, string Valor)[] itens)
    {
        var sb = new StringBuilder();
        sb.Append("<div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>");
        foreach (var item in itens)
        {
            sb.Append($"<p style='margin: 5px 0;'><strong>{item.Label}:</strong> {item.Valor}</p>");
        }
        sb.Append("</div>");
        return sb.ToString();
    }

    public string GerarBoasVindasEmpresa(string razaoSocial) =>
        MontarLayoutBase("Bem-vindo ao Sistema de Condomínio!", razaoSocial, "Sua empresa foi cadastrada com sucesso.");

    public string GerarEmpresaAlterada(string razaoSocial) =>
        MontarLayoutBase("Empresa alterada no Sistema de Condomínio!!", razaoSocial, "Seu cadastro de empresa foi alterada.");

    public string GerarBoasVindasUsuario(string nomeUsuario, string senhaTemporaria)
    {
        var extra = GerarQuadroInformativo(("Usuário", nomeUsuario), ("Senha Temporária", $"<span style='color: #e74c3c; font-weight: bold;'>{senhaTemporaria}</span>"));
        return MontarLayoutBase("Bem-vindo ao Sistema de Condomínio!", nomeUsuario, "Seu acesso foi criado com sucesso. Utilize os dados abaixo para realizar seu primeiro login:", extra);
    }

    public string GerarUsuarioAlterado(string nomeUsuario)
    {
        var extra = GerarQuadroInformativo(("Usuário", nomeUsuario));
        return MontarLayoutBase("Usuário alterado no Sistema de Condomínio!", nomeUsuario, "Seu cadastro de usuário foi alterado.", extra);
    }

    public string GerarBoasVindasMorador(string nome) =>
        MontarLayoutBase("Bem-vindo ao Sistema de Condomínio!", nome, "Você foi cadastrado(a) como morador(a) com sucesso.");

    public string GerarMoradorAlterado(string nome) =>
        MontarLayoutBase("Morador alterado no Sistema de Condomínio!!", nome, "Seu cadastro de morador foi alterado.");

    public string GerarRedefinicaoSenha(string nomeUsuario, string link)
    {
        string botao = $@"<div style='text-align: center; margin: 30px 0;'>
                            <a href='{link}' style='background-color: #3498db; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px;'>Redefinir Senha</a>
                          </div>";
        return MontarLayoutBase("Redefinição de Senha", nomeUsuario, "Clique no botão abaixo para criar uma nova senha:", botao);
    }
}