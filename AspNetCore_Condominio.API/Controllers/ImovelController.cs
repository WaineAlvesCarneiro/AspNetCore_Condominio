using AspNetCore_Condominio.API.Controllers.ApiBase;
using AspNetCore_Condominio.Application.Features.Imoveis.Commands.Create;
using AspNetCore_Condominio.Application.Features.Imoveis.Commands.Delete;
using AspNetCore_Condominio.Application.Features.Imoveis.Commands.Update;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAll;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAllPaged;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ImovelController(IMediator mediator) : ApiBaseController
{
    [Authorize(Roles = "Sindico, Porteiro")]
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] long? empresaId = null)
    {
        var result = await mediator.Send(new GetAllQueryImovel(
            EmpresaId: Convert.ToInt64(empresaId)));

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
        [FromQuery] string? direction = "ASC",
        [FromQuery] long? empresaId = null,
        [FromQuery] string? bloco = null,
        [FromQuery] string? apartamento = null)
    {
        var query = new GetAllPagedQueryImovel(
                Page: page,
                PageSize: pageSize,
                SortBy: sortBy ?? "Id",
                Direction: direction ?? "ASC",
                EmpresaId: empresaId,
                Bloco: bloco,
                Apartamento: apartamento
            );

        var result = await mediator.Send(query);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Roles = "Sindico, Porteiro")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await mediator.Send(new GetByIdQueryImovel(id));

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : NotFound(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Roles = "Sindico")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCommandImovel command)
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
    public async Task<IActionResult> Put(long id, [FromBody] UpdateCommandImovel command)
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
        var result = await mediator.Send(new DeleteCommandImovel(id));

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }
}