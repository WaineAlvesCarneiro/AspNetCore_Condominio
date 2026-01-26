using AspNetCore_Condominio.API_Controller.Controllers.ApiBase;
using AspNetCore_Condominio.Application.Features.Imoveis.Commands.Delete;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.Create;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.Delete;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.Update;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAll;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllPaged;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.API_Controller.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "AdminPolicy")]
public class MoradorController(IMediator mediator) : ApiBaseController
{
    [Authorize(Roles = "Sindico, Porteiro")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await mediator.Send(new GetAllQueryMorador(UserEmpresaId));

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Roles = "Sindico, Porteiro")]
    [HttpGet("paginado")]
    public async Task<IActionResult> GetAllPagedAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "Id",
        [FromQuery] string? sortDescending = "ASC",
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetAllPagedQueryMorador(
            UserEmpresaId: UserEmpresaId,
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

    [Authorize(Roles = "Sindico, Porteiro")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await mediator.Send(new GetByIdQueryMorador(id, UserEmpresaId));

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : NotFound(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Roles = "Sindico")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCommandMorador command)
    {
        command.EmpresaId = UserEmpresaId;

        var result = await mediator.Send(command);

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
    public async Task<IActionResult> Put(long id, [FromBody] UpdateCommandMorador command)
    {
        if (id != command.Id)
        {
            return BadRequest("O ID da URL não corresponde ao ID do corpo da requisição.");
        }
        command.EmpresaId = UserEmpresaId;

        var result = await mediator.Send(command);

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Roles = "Sindico")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        if (IsSindico)
        {
            var result = await mediator.Send(new DeleteCommandMorador(id, UserEmpresaId));

            return result.Sucesso
                ? NoContent()
                : BadRequest(new { sucesso = false, erro = result.Mensagem });
        }

        return Forbid();
    }
}