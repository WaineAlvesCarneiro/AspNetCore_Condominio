using AspNetCore_Condominio.Application.Features.Auth;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
using AspNetCore_Condominio.Application.Features.Auth.Commands.DefinirSenha;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Delete;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Update;
using AspNetCore_Condominio.Application.Features.Auth.Queries.GetAll;
using AspNetCore_Condominio.Application.Features.Auth.Queries.GetAllPaged;
using AspNetCore_Condominio.Application.Features.Auth.Queries.GetById;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Configurations.ServicesJWT;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IMediator mediator, TokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] AuthLoginRequest request, CancellationToken cancellationToken)
    {
        var query = new AuthLoginQuery { Username = request.Username, Password = request.Password };
        var user = await mediator.Send(query, cancellationToken);

        if (user == null)
            return Unauthorized();

        if (user.EmpresaAtiva != TipoEmpresaAtivo.Ativo)
            return BadRequest(new { sucesso = false, erro = "O acesso para este Condomínio está suspenso. Procure o suporte." });

        if (user.Ativo != TipoUserAtivo.Ativo)
            return BadRequest(new { sucesso = false, erro = "Seu usuário está inativo." });

        var token = tokenService.GenerateToken(
            user.UserName,
            (TipoRole)user.Role,
            user.EmpresaId,
            user.PrimeiroAcesso,
            (TipoUserAtivo)user.Ativo,
            (TipoEmpresaAtivo)user.EmpresaAtiva
        );

        return Ok(new { token, sucesso = true, primeiroAcesso = user.PrimeiroAcesso });
    }

    [Authorize(Policy = "PermitirTrocaSenha")]
    [HttpPost("definir-senha-permanente")]
    public async Task<IActionResult> DefinirSenha([FromBody] DefinirSenhaRequest request, CancellationToken cancellationToken)
    {
        var username = User.Identity?.Name;

        if (string.IsNullOrEmpty(username))
            return Unauthorized(new { sucesso = false, erro = "Sessão inválida." });

        var command = new DefinirSenhaCommand
        {
            UserName = username,
            NovaSenha = request.NovaSenha
        };

        var result = await mediator.Send(command, cancellationToken);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados, mensagem = result.Mensagem })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    public record DefinirSenhaRequest(string NovaSenha);

    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Roles = "Suporte, Sindico")]
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken,
        [FromQuery] long? empresaId = null)
    {
        var result = await mediator.Send(new GetAllQueryAuthUser(
             EmpresaId: Convert.ToInt64(empresaId)), cancellationToken);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Roles = "Suporte, Sindico")]
    [HttpGet("paginado")]
    public async Task<IActionResult> GetAllPagedAsync(CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "Id",
        [FromQuery] string? direction = "ASC",
        [FromQuery] long? empresaId = null,
        [FromQuery] string? userName = null)
    {
        var query = new GetAllPagedQueryAuthUser(
            Page: page,
            PageSize: pageSize,
            SortBy: sortBy ?? "Id",
            Direction: direction ?? "ASC",
            EmpresaId: empresaId,
            UserName: userName);

        var result = await mediator.Send(query, cancellationToken);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Roles = "Suporte, Sindico")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetByIdQueryAuthUser(id), cancellationToken);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : NotFound(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Roles = "Suporte")]
    [HttpPost("criar-usuario")]
    public async Task<IActionResult> Post([FromBody] CreateCommandAuthUser command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.Sucesso)
            return BadRequest(new { sucesso = false, erro = result.Mensagem });

        return CreatedAtAction(nameof(GetById), new { id = result.Dados!.Id }, new
        {
            sucesso = true,
            dados = result.Dados
        });
    }

    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Roles = "Suporte, Sindico")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateCommandAuthUser command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("O ID da URL não corresponde ao ID do corpo da requisição.");

        if (!User.IsSuporte())
            command.Role = null;

        var result = await mediator.Send(command, cancellationToken);

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Roles = "Suporte")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteCommandAuthUser(id), cancellationToken);

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }
}