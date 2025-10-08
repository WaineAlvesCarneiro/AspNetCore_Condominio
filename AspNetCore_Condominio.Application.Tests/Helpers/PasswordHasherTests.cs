using AspNetCore_Condominio.Application.Helpers;

namespace AspNetCore_Condominio.Application.Tests.Helpers;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_DeveCriarHashVerificavel()
    {
        const string senhaOriginal = "MinhaSenhaForte123";
        var hash = PasswordHasher.HashPassword(senhaOriginal);

        Assert.NotEqual(senhaOriginal, hash);
        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.True(PasswordHasher.VerifyPassword(senhaOriginal, hash));
    }

    [Fact]
    public void VerifyPassword_SenhaIncorreta_DeveRetornarFalse()
    {
        const string senhaCorreta = "SenhaCorreta123";
        const string senhaIncorreta = "SenhaErrada321";
        var hash = PasswordHasher.HashPassword(senhaCorreta);
        var resultado = PasswordHasher.VerifyPassword(senhaIncorreta, hash);

        Assert.False(resultado);
    }
}