using Microsoft.AspNetCore.Mvc;
using AspNetCore_Condominio.Domain.Enums;

namespace AspNetCore_Condominio.API.Controllers;

[ApiController]
[Route("[controller]")]
public class EnumsController : ControllerBase
{
    [HttpGet("tipo-condominio")]
    public IActionResult GetTipoCondominio()
    {
        var options = Enum.GetValues(typeof(TipoCondominio))
            .Cast<TipoCondominio>()
            .Select(e => new
            {
                Value = (int)e,
                Label = e.ToString()
            })
            .ToList();

        return Ok(options);
    }
}