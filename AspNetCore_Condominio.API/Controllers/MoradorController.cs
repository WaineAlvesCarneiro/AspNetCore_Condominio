using AspNetCore_Condominio.API.Controllers.ApiBase;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.Create;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.Delete;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.Update;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAll;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllPaged;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.API.Controllers;

[ApiController]
[Route("[controller]")]
public class MoradorController(IMediator mediator) : ApiBaseController
{
    [Authorize(Roles = "Sindico, Porteiro")]
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken,
        [FromQuery] long? empresaId = null)
    {
        var result = await mediator.Send(new GetAllQueryMorador(
            EmpresaId: Convert.ToInt64(empresaId)), cancellationToken);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Roles = "Sindico, Porteiro")]
    [HttpGet("paginado")]
    public async Task<IActionResult> GetAllPagedAsync(CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "Id",
        [FromQuery] string? direction = "ASC",
        [FromQuery] long? empresaId = null,
        [FromQuery] string? nome = null)
    {
        var query = new GetAllPagedQueryMorador(
            Page: page,
            PageSize: pageSize,
            SortBy: sortBy ?? "Id",
            Direction: direction ?? "ASC",
            EmpresaId: empresaId,
            Nome: nome);

        var result = await mediator.Send(query, cancellationToken);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Roles = "Sindico, Porteiro")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetByIdQueryMorador(id), cancellationToken);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : NotFound(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Roles = "Sindico")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCommandMorador command, CancellationToken cancellationToken)
    {
        command.EmpresaId = UserEmpresaId;

        var result = await mediator.Send(command, cancellationToken);

        if (!result.Sucesso)
            return BadRequest(new { sucesso = false, erro = result.Mensagem });

        return CreatedAtAction(nameof(GetById), new { id = result.Dados!.Id }, new
        {
            sucesso = true,
            dados = result.Dados
        });
    }

    [Authorize(Roles = "Sindico")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(long id, [FromBody] UpdateCommandMorador command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("O ID da URL não corresponde ao ID do corpo da requisição.");
        }
        command.EmpresaId = UserEmpresaId;

        var result = await mediator.Send(command, cancellationToken);

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Roles = "Sindico")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteCommandMorador(id), cancellationToken);

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }
}