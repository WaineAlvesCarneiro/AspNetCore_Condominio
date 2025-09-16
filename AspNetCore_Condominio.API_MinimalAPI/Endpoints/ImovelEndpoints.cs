using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Imoveis.Commands.CreateImovel;
using AspNetCore_Condominio.Application.Features.Imoveis.Commands.UpdateImovel;
using AspNetCore_Condominio.Application.Features.Imoveis.DeleteImovel;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAllImoveis;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetAllPagedImoveis;
using AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetImovelById;
using AspNetCore_Condominio.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.API_MinimalAPI.Endpoints;

public static class ImovelEndpoints
{
    public static void MapImovelEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/imovel")
            .WithTags("Imovel");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var query = new GetAllImoveisQuery();
            var result = await mediator.Send(query);
            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, mensagem = result.Mensagem });
        })
            .WithName("GetAllImoveis")
            .WithOpenApi()
            .WithSummary("Lista todos os imóveis.")
            .WithDescription("Retorna uma lista completa de todos os imóveis disponíveis no sistema.")
            .Produces<List<ImovelDto>>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("AdminPolicy")
            .RequireCors()
            .CacheOutput();

        group.MapGet("/paginado", async ([AsParameters] GetAllPagedImoveisQuery query, ISender sender) =>
        {
            var result = await sender.Send(query);
            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, mensagem = result.Mensagem });
        })
            .WithName("GetAllPagedImoveis")
            .WithOpenApi()
            .WithSummary("Lista os imóveis com paginação.")
            .WithDescription("Retorna uma lista paginada de imóveis.")
            .Produces<PagedResult<ImovelDto>>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("AdminPolicy")
            .RequireCors()
            .CacheOutput();

        group.MapGet("/{id}", async (int id, IMediator mediator) =>
        {
            var query = new GetImovelByIdQuery(id);
            var result = await mediator.Send(query);
            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, mensagem = result.Mensagem });
        })
            .WithName("GetImovelById")
            .WithOpenApi()
            .WithSummary("Retorna um imóvel por ID.")
            .WithDescription("Retorna um imóvel por ID disponível no sistema.")
            .Produces<ImovelDto>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("AdminPolicy")
            .RequireCors()
            .CacheOutput();

        group.MapPost("/", async (CreateImovelCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result.Sucesso
                ? Results.CreatedAtRoute("GetImovelById", new { id = result.Dados.Id }, result.Dados)
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        })
            .WithName("CreateImovel")
            .WithOpenApi()
            .WithSummary("Cadastra um novo imóvel.")
            .WithDescription("Cadastra um novo imóvel no sistema.")
            .Produces<ImovelDto>(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("AdminPolicy")
            .RequireCors()
            .CacheOutput();

        group.MapPut("/{id}", async (int id, [FromBody] UpdateImovelCommand command, ISender sender) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest(new { sucesso = false, erro = "O ID da URL não corresponde ao ID do corpo da requisição." });
            }
            var result = await sender.Send(command);
            return result.Sucesso
                ? Results.NoContent()
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        })
            .WithName("UpdateImovel")
            .WithOpenApi()
            .WithSummary("Atualiza os dados de um imóvel existente.")
            .WithDescription("Atualiza um novo imóvel no sistema.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("AdminPolicy")
            .RequireCors()
            .CacheOutput();

        group.MapDelete("/{id}", async (int id, IMediator mediator) =>
        {
            var command = new DeleteImovelCommand(id);
            var result = await mediator.Send(command);
            return result.Sucesso
                ? Results.NoContent()
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        })
            .WithName("DeleteImovel")
            .WithOpenApi()
            .WithSummary("Deleta um imóvel por ID.")
            .WithDescription("Deleta um imóvel no sistema.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("AdminPolicy")
            .RequireCors()
            .CacheOutput();
    }
}

