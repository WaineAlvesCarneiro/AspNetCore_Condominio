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
public class MoradorController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await mediator.Send(new GetAllQueryMorador());

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [HttpGet("paginado")]
    public async Task<IActionResult> GetAllPagedAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "Id",
        [FromQuery] bool sortDescending = false,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetAllPagedQueryMorador(
            Page: page,
            PageSize: pageSize,
            SortBy: sortBy,
            SortDescending: sortDescending,
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await mediator.Send(new GetByIdQueryMorador(id));

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : NotFound(new { sucesso = false, erro = result.Mensagem });
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCommandMorador command)
    {
        command.EmpresaId = 1;
        var result = await mediator.Send(command);

        if (!result.Sucesso)
            return BadRequest(new { sucesso = false, erro = result.Mensagem });

        return CreatedAtAction(nameof(GetById), new { id = result.Dados!.Id }, new
        {
            sucesso = true,
            dados = result.Dados
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(long id, [FromBody] UpdateCommandMorador command)
    {
        command.EmpresaId = 1;
        if (id != command.Id)
        {
            return BadRequest("O ID da URL não corresponde ao ID do corpo da requisição.");
        }

        var result = await mediator.Send(command);

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await mediator.Send(new DeleteCommandMorador(id));

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }
}