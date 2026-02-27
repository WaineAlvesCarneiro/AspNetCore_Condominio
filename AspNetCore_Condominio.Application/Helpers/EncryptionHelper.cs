using System.Security.Cryptography;
using System.Text;

namespace AspNetCore_Condominio.Application.Helpers;

public static class EncryptionHelper
{
    // No mercado, esta chave NUNCA fica no código. 
    // Ela deve vir de uma Variável de Ambiente ou Azure Key Vault.
    // A chave precisa ter exatamente 32 caracteres para AES-256.
    private static readonly string SecurityKey = "C0nd0m1n10Cl0udS3cur1tyK3y_2026!";

    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;

        using Aes aesAlg = Aes.Create();
        // Derivando uma chave de 32 bytes a partir da nossa string
        aesAlg.Key = Encoding.UTF8.GetBytes(SecurityKey);
        aesAlg.IV = new byte[16]; // Para um sistema simples, IV zerado funciona, mas no mercado usamos IV aleatório por mensagem.

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msEncrypt = new MemoryStream();
        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
        }
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return cipherText;

        try
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(SecurityKey);
            aesAlg.IV = new byte[16];

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText));
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
        catch
        {
            // Se der erro (ex: chave errada), retorna o texto original ou loga o erro.
            return cipherText;
        }
    }
}