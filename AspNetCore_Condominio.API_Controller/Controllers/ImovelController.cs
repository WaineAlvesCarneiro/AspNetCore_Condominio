using AspNetCore_Condominio.Application.Features.Imoveis.Commands.CreateImovel;
using AspNetCore_Condominio.Application.Features.Imoveis.Commands.UpdateImovel;
using AspNetCore_Condominio.Application.Features.Imoveis.DeleteImovel;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAllImoveis;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAllPagedImoveis;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetImovelById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.API_Controller.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "AdminPolicy")]
public class ImovelController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lista todos os imóveis.
    /// </summary>
    /// <response code="200">Lista retornada com sucesso.</response>
    /// <response code="400">Ocorreu um erro na requisição.</response>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await mediator.Send(new GetAllImoveisQuery());

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    /// <summary>
    /// Lista os imóveis com paginação.
    /// </summary>
    /// <param name="query">Parâmetros de paginação e ordenação.</param>
    /// <response code="200">Lista paginada retornada com sucesso.</response>
    /// <response code="400">Ocorreu um erro na requisição.</response>
    [HttpGet("paginado")]
    public async Task<IActionResult> GetAllPagedAsync([FromQuery] GetAllPagedImoveisQuery query)
    {
        var result = await mediator.Send(query);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    /// <summary>
    /// Retorna um imóvel por ID.
    /// </summary>
    /// <response code="200">Imóvel encontrado.</response>
    /// <response code="404">Imóvel não encontrado.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetImovelByIdQuery(id));

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : NotFound(new { sucesso = false, erro = result.Mensagem });
    }

    /// <summary>
    /// Cadastra um novo imóvel.
    /// </summary>
    /// <response code="201">Imóvel criado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateImovelCommand command)
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
    /// Atualiza os dados de um imóvel existente.
    /// </summary>
    /// <response code="204">Imóvel atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos ou ID da URL não corresponde ao corpo da requisição.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateImovelCommand command)
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
    /// Deleta um imóvel por ID.
    /// </summary>
    /// <response code="204">Imóvel deletado com sucesso.</response>
    /// <response code="400">Imóvel não encontrado.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteImovelCommand(id));

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }
}