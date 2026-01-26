using System.Security.Claims;

namespace AspNetCore_Condominio.API_MinimalAPI.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetEmpresaId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("EmpresaId")?.Value;
        return long.TryParse(claim, out var id) ? id : 0;
    }

    public static string GetUserName(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    public static string GetUserRole(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
}