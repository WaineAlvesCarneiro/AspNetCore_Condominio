using AspNetCore_Condominio.Domain.Entities.Auth;

namespace AspNetCore_Condominio.Domain.Tests.Entities.Auth;

public class AuthUserTests
{
    [Fact]
    public void VerificarSenha_SenhaCorreta_DeveRetornarVerdadeiro()
    {
        const string senhaValida = "SenhaSecreta123";
        var hash = BCrypt.Net.BCrypt.HashPassword(senhaValida);
        var user = new AuthUser { UserName = "teste", PasswordHash = hash, Role = "Suporte" };
        var resultado = user.VerificarSenha(senhaValida);

        Assert.True(resultado);
    }

    [Fact]
    public void VerificarSenha_SenhaIncorreta_DeveRetornarFalso()
    {
        const string senhaCorreta = "SenhaSecreta123";
        var hash = BCrypt.Net.BCrypt.HashPassword(senhaCorreta);
        var user = new AuthUser { UserName = "teste", PasswordHash = hash, Role = "Suporte" };

        const string senhaErrada = "SenhaErrada";
        var resultado = user.VerificarSenha(senhaErrada);

        Assert.False(resultado);
    }
}