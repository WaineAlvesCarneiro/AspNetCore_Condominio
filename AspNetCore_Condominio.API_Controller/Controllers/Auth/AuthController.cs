using AspNetCore_Condominio.Application.Features.Auth;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Delete;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Update;
using AspNetCore_Condominio.Application.Features.Auth.Queries.GetAll;
using AspNetCore_Condominio.Application.Features.Auth.Queries.GetAllPaged;
using AspNetCore_Condominio.Application.Features.Auth.Queries.GetById;
using AspNetCore_Condominio.Configurations.ServicesJWT;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
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

        var token = _tokenService.GenerateToken(user.UserName, (TipoRole)user.Role, user.EmpresaId);
        return Ok(new { token });
    }

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
    [HttpPost]
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
    public async Task<IActionResult> Put(long id, [FromBody] UpdateCommandAuthUser command)
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