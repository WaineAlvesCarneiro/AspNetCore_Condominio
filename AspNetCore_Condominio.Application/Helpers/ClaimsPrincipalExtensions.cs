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

    public static long GetEmpresaId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("empresaId")?.Value;
        return long.TryParse(claim, out var id) ? id : 0;
    }

    public static string GetUserName(this ClaimsPrincipal user) =>
    user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    public static TipoUserAtivo? GetUserStatus(this ClaimsPrincipal user)
    {
        var status = user.FindFirst("statusAtivo")?.Value;
        return Enum.TryParse<TipoUserAtivo>(status, out var result) ? result : null;
    }

    public static TipoEmpresaAtivo? GetEmpresaStatus(this ClaimsPrincipal user)
    {
        var status = user.FindFirst("empresaAtiva")?.Value;
        return Enum.TryParse<TipoEmpresaAtivo>(status, out var result) ? result : null;
    }

    public static TipoRole? GetUserRole(this ClaimsPrincipal user)
    {
        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        return Enum.TryParse<TipoRole>(role, out var result) ? result : null;
    }
}