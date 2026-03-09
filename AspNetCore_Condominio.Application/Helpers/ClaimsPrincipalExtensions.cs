using AspNetCore_Condominio.Domain.Constants;
using AspNetCore_Condominio.Domain.Enums;
using System.Security.Claims;

namespace AspNetCore_Condominio.Application.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static bool IsSuporte(this ClaimsPrincipal user) =>
        user.IsInRole(TipoRole.Suporte.ToString());

    public static bool IsSindico(this ClaimsPrincipal user) =>
        user.IsInRole(TipoRole.Sindico.ToString());

    public static bool IsPorteiro(this ClaimsPrincipal user) =>
        user.IsInRole(TipoRole.Porteiro.ToString());

    public static long GetEmpresaId(this ClaimsPrincipal user) => 
        long.TryParse(user.FindFirst(AuthClaims.EmpresaId)?.Value, out var id) ? id : 0;

    public static string GetUserName(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    public static TipoUserAtivo? GetUserStatus(this ClaimsPrincipal user) =>
        user.Claims.FirstOrDefault(c => c.Type == AuthClaims.StatusAtivo) is { } statusClaim
            && Enum.TryParse<TipoUserAtivo>(statusClaim.Value, out var result) ? result : null;

    public static TipoEmpresaAtivo? GetEmpresaStatus(this ClaimsPrincipal user) =>
        user.Claims.FirstOrDefault(c => c.Type == AuthClaims.EmpresaAtiva) is { } statusClaim
            && Enum.TryParse<TipoEmpresaAtivo>(statusClaim.Value, out var result) ? result : null;

    public static TipoRole? GetUserRole(this ClaimsPrincipal user) =>
        user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role) is { } roleClaim
            && Enum.TryParse<TipoRole>(roleClaim.Value, out var result) ? result : null;
}