using AspNetCore_Condominio.Application.Features.Auth;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
using AspNetCore_Condominio.Application.Features.Auth.Commands.DefinirSenha;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Delete;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Update;
using AspNetCore_Condominio.Application.Features.Auth.Queries.GetAll;
using AspNetCore_Condominio.Application.Features.Auth.Queries.GetAllPaged;
using AspNetCore_Condominio.Application.Features.Auth.Queries.GetById;
using AspNetCore_Condominio.Configurations.ServicesJWT;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.API_Controller.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IMediator mediator, TokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] AuthLoginRequest request)
    {
        var query = new AuthLoginQuery { Username = request.Username, Password = request.Password };
        var user = await mediator.Send(query);

        if (user == null)
            return Unauthorized(new { mensagem = "Usuário ou senha inválidos" });

        var token = tokenService.GenerateToken(
            user.UserName,
            (TipoRole)user.Role,
            user.EmpresaId,
            user.PrimeiroAcesso
        );

        return Ok(new { token, sucesso = true, primeiroAcesso = user.PrimeiroAcesso });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AllowAnonymous]
    [HttpPost("definir-senha-permanente")]
    public async Task<IActionResult> DefinirSenha([FromBody] DefinirSenhaRequest request)
    {
        var username = User.Identity?.Name;

        if (string.IsNullOrEmpty(username))
            return Unauthorized(new { sucesso = false, erro = "Sessão inválida." });

        var command = new DefinirSenhaCommand
        {
            UserName = username,
            NovaSenha = request.NovaSenha
        };

        var result = await mediator.Send(command);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados, mensagem = result.Mensagem })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    public record DefinirSenhaRequest(string NovaSenha);


    [Authorize(Roles = "Suporte")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await mediator.Send(new GetAllQueryAuthUser());

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Roles = "Suporte")]
    [HttpGet("paginado")]
    public async Task<IActionResult> GetAllPagedAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "Id",
        [FromQuery] string? sortDescending = "ASC",
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetAllPagedQueryAuthUser(
            Page: page,
            PageSize: pageSize,
            SortBy: sortBy,
            SortDescending: sortDescending!,
            SearchTerm: searchTerm);

        var result = await mediator.Send(query);

        return result.Sucesso
            ? Ok(new
            {
                sucesso = true,
                dados = result.Dados
            })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Roles = "Suporte")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetByIdQueryAuthUser(id));

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : NotFound(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Roles = "Suporte")]
    [HttpPost("criar-usuario")]
    public async Task<IActionResult> Post([FromBody] CreateCommandAuthUser command)
    {
        var result = await mediator.Send(command);

        if (!result.Sucesso)
            return BadRequest(new { sucesso = false, erro = result.Mensagem });

        return CreatedAtAction(nameof(GetById), new { id = result.Dados!.Id }, new
        {
            sucesso = true,
            dados = result.Dados
        });
    }

    [Authorize(Roles = "Suporte")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateCommandAuthUser command)
    {
        if (id != command.Id)
        {
            return BadRequest("O ID da URL não corresponde ao ID do corpo da requisição.");
        }

        var result = await mediator.Send(command);

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Roles = "Suporte")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteCommandAuthUser(id));

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }
}