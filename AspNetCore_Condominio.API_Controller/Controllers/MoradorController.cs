using AspNetCore_Condominio.Application.Features.Moradores.Commands.CreateMorador;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.UpdateMorador;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.DeleteMorador;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllMoradores;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllPagedMoradores;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetMoradorById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.API_Controller.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "AdminPolicy")]
public class MoradorController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lista todos os morador.
    /// </summary>
    /// <response code="200">Lista retornada com sucesso.</response>
    /// <response code="400">Ocorreu um erro na requisição.</response>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await mediator.Send(new GetAllMoradoresQuery());

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    /// <summary>
    /// Lista os moradores com paginação.
    /// </summary>
    /// <param name="query">Parâmetros de paginação e ordenação.</param>
    /// <response code="200">Lista paginada retornada com sucesso.</response>
    /// <response code="400">Ocorreu um erro na requisição.</response>
    [HttpGet("paginado")]
    public async Task<IActionResult> GetAllPagedAsync([FromQuery] GetAllPagedMoradoresQuery query)
    {
        var result = await mediator.Send(query);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    /// <summary>
    /// Retorna um morador por ID.
    /// </summary>
    /// <response code="200">Morador encontrado.</response>
    /// <response code="404">Morador não encontrado.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetMoradorByIdQuery(id));

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : NotFound(new { sucesso = false, erro = result.Mensagem });
    }

    /// <summary>
    /// Cadastra um novo morador.
    /// </summary>
    /// <response code="201">Morador criado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateMoradorCommand command)
    {
        var result = await mediator.Send(command);

        if (!result.Sucesso)
            return BadRequest(new { sucesso = false, erro = result.Mensagem });

        return CreatedAtAction(nameof(GetById), new { id = result.Dados.Id }, new
        {
            sucesso = true,
            dados = result.Dados
        });
    }

    /// <summary>
    /// Atualiza os dados de um morador existente.
    /// </summary>
    /// <response code="204">Morador atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos ou ID da URL não corresponde ao corpo da requisição.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateMoradorCommand command)
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

    /// <summary>
    /// Deleta um morador por ID.
    /// </summary>
    /// <response code="204">Morador deletado com sucesso.</response>
    /// <response code="400">Morador não encontrado.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteMoradorCommand(id));

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }
}