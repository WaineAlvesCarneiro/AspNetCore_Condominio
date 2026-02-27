using BCrypt.Net;

namespace AspNetCore_Condominio.Application.Helpers;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public static string GerarSenhaAleatoria(int tamanho = 8)
    {
        const string caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Sem '0', 'O', '1', 'l' para evitar confusão
        var random = new Random();
        return new string(Enumerable.Repeat(caracteres, tamanho)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}