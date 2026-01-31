using System.Security.Claims;
using AspNetCore_Condominio.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.API_Controller.Controllers.ApiBase;

public abstract class ApiBaseController : ControllerBase
{
    protected long UserEmpresaId =>
        long.TryParse(User.FindFirst("EmpresaId")?.Value, out var id) ? id : 0;

    protected string UserName => User.Identity?.Name ?? string.Empty;

    protected TipoRole? UserRole =>
        Enum.TryParse<TipoRole>(User.FindFirst(ClaimTypes.Role)?.Value, out var role) ? role : null;

    protected bool IsPrimeiroAcesso =>
        bool.TryParse(User.FindFirst("primeiroAcesso")?.Value, out var primeiro) && primeiro;

    protected bool IsSuporte => UserRole == TipoRole.Suporte;
    protected bool IsSindico => UserRole == TipoRole.Sindico;
    protected bool IsPorteiro => UserRole == TipoRole.Porteiro;
}