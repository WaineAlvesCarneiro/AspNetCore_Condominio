using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.API_Controller.Controllers.ApiBase;

[ApiController]
public abstract class ApiBaseController : ControllerBase
{
    protected long UserEmpresaId
    {
        get
        {
            var claim = User.FindFirst("EmpresaId")?.Value;
            return long.TryParse(claim, out var id) ? id : 0;
        }
    }

    protected string UserName => User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    protected string UserRole => User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

    protected bool IsSuporte => UserRole == "Suporte";
    protected bool IsSindico => UserRole == "Sindico";
    protected bool IsPorteiro => UserRole == "Porteiro";
}