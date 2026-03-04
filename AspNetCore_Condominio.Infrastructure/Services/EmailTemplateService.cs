using AspNetCore_Condominio.Domain.Interfaces;

namespace AspNetCore_Condominio.Infrastructure.Services;

public class EmailTemplateService : IEmailTemplateService
{
    public string GerarBoasVindasEmpresa(string razaoSocial)
    {
        return $@"
        <div style='font-family: sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px;'>
            <h2 style='color: #2c3e50;'>Bem-vindo ao Sistema de Condomínio!</h2>
            <p>Olá <strong>{razaoSocial}</strong>,</p>
            <p>Sua empresa foi cradastrada com sucesso.</p>
            <p style='color: #7f8c8d; font-size: 0.9em;'>Por segurança, você deverá alterar esta senha no seu primeiro acesso.</p>
            <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
            <p style='font-size: 0.8em; color: #bdc3c7;'>Este é um e-mail automático, por favor não responda.</p>
        </div>";
    }

    public string GerarEmpresaAlterada(string razaoSocial)
    {
        return $@"
        <div style='font-family: sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px;'>
            <h2 style='color: #2c3e50;'>Empresa alterada no Sistema de Condomínio!</h2>
            <p>Olá <strong>{razaoSocial}</strong>,</p>
            <p>Sua empresa foi alterada.</p>
            <p style='color: #7f8c8d; font-size: 0.9em;'>Por segurança, você deverá alterar esta senha no seu primeiro acesso.</p>
            <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
            <p style='font-size: 0.8em; color: #bdc3c7;'>Este é um e-mail automático, por favor não responda.</p>
        </div>";
    }

    public string GerarBoasVindasUsuario(string nomeUsuario, string senhaTemporaria)
    {
        return $@"
        <div style='font-family: sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px;'>
            <h2 style='color: #2c3e50;'>Bem-vindo ao Sistema de Condomínio!</h2>
            <p>Olá <strong>{nomeUsuario}</strong>,</p>
            <p>Seu acesso foi criado com sucesso. Utilize os dados abaixo para realizar seu primeiro login:</p>
            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                <p style='margin: 5px 0;'><strong>Usuário:</strong> {nomeUsuario}</p>
                <p style='margin: 5px 0;'><strong>Senha Temporária:</strong> <span style='color: #e74c3c; font-weight: bold;'>{senhaTemporaria}</span></p>
            </div>
            <p style='color: #7f8c8d; font-size: 0.9em;'>Por segurança, você deverá alterar esta senha no seu primeiro acesso.</p>
            <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
            <p style='font-size: 0.8em; color: #bdc3c7;'>Este é um e-mail automático, por favor não responda.</p>
        </div>";
    }

    public string GerarUsuarioAlterado(string nomeUsuario)
    {
        return $@"
        <div style='font-family: sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px;'>
            <h2 style='color: #2c3e50;'>Usuário alterado no Sistema de Condomínio!</h2>
            <p>Olá <strong>{nomeUsuario}</strong>,</p>
            <p>Usuário foi alterado(a).</p>
            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                <p style='margin: 5px 0;'><strong>Usuário:</strong> {nomeUsuario}</p>
            </div>
            <p style='color: #7f8c8d; font-size: 0.9em;'>Por segurança, você deverá alterar esta senha no seu primeiro acesso.</p>
            <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
            <p style='font-size: 0.8em; color: #bdc3c7;'>Este é um e-mail automático, por favor não responda.</p>
        </div>";
    }

    public string GerarBoasVindasMorador(string nome)
    {
        return $@"
        <div style='font-family: sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px;'>
            <h2 style='color: #2c3e50;'>Bem-vindo ao Sistema de Condomínio!</h2>
            <p>Olá <strong>{nome}</strong>,</p>
            <p>Você foi cadastrado(a) como  morador(a) com sucesso.</p>
            <p style='color: #7f8c8d; font-size: 0.9em;'>Por segurança, você deverá alterar esta senha no seu primeiro acesso.</p>
            <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
            <p style='font-size: 0.8em; color: #bdc3c7;'>Este é um e-mail automático, por favor não responda.</p>
        </div>";
    }

    public string GerarMoradorAlterado(string nome)
    {
        return $@"
        <div style='font-family: sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px;'>
            <h2 style='color: #2c3e50;'>Empresa alterada no Sistema de Condomínio!</h2>
            <p>Olá <strong>{nome}</strong>,</p>
            <p>Morador foi alterado(a).</p>
            <p style='color: #7f8c8d; font-size: 0.9em;'>Por segurança, você deverá alterar esta senha no seu primeiro acesso.</p>
            <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
            <p style='font-size: 0.8em; color: #bdc3c7;'>Este é um e-mail automático, por favor não responda.</p>
        </div>";
    }

    public string GerarRedefinicaoSenha(string nomeUsuario, string link)
    {
        // Implementação futura...
        return "";
    }
}