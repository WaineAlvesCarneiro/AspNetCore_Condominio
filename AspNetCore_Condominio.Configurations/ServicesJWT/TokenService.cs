using AspNetCore_Condominio.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspNetCore_Condominio.Configurations.ServicesJWT;

public class TokenService(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateToken(string username, TipoRole role, long? empresaId, bool primeiroAcesso, TipoUserAtivo userAtivo, TipoEmpresaAtivo empresaAtiva)
    {
        var claims = GerarClaims(username, role, primeiroAcesso, userAtivo, empresaAtiva);

        if (empresaId.HasValue) claims.Add(new Claim("empresaId", empresaId.Value.ToString()));

        var tokenDescriptor = GerarTokenDescriptor(_configuration, claims);

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static List<Claim> GerarClaims(string username, TipoRole role, bool primeiroAcesso, TipoUserAtivo userAtivo, TipoEmpresaAtivo empresaAtiva)
    {
        return
        [
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, role.ToString()),
            new("primeiroAcesso", primeiroAcesso.ToString().ToLower()),
            new("statusAtivo", userAtivo.ToString()),
            new("empresaAtiva", empresaAtiva.ToString())
        ];
    }

    private static SecurityTokenDescriptor GerarTokenDescriptor(IConfiguration _configuration, List<Claim> claims)
    {
        return new SecurityTokenDescriptor
        {
            NotBefore = DateTime.UtcNow,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            Audience = _configuration["Jwt:Audience"],
            Issuer = _configuration["Jwt:Issuer"],
            SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!)),
                    SecurityAlgorithms.HmacSha256Signature)
        };
    }
}
