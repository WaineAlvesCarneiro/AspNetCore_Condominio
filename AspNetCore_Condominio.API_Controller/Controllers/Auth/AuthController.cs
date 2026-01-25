using AspNetCore_Condominio.Application.Features.Auth;
using AspNetCore_Condominio.Configurations.ServicesJWT;
using AspNetCore_Condominio.Domain.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.API_Controller.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IMediator mediator, TokenService tokenService) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly TokenService _tokenService = tokenService;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] AuthLoginRequest request)
    {
        var query = new AuthLoginQuery { Username = request.Username, Password = request.Password };
        var user = await _mediator.Send(query);

        if (user == null)
        {
            return Unauthorized();
        }

        var token = _tokenService.GenerateToken(user.UserName, user.Role);
        return Ok(new { token });
    }
}