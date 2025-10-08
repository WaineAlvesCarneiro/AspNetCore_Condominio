using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.CreateMorador;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.DeleteMorador;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.UpdateMorador;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllMoradores;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllPagedMoradores;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetMoradorById;
using AspNetCore_Condominio.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_Condominio.API_MinimalAPI.Endpoints;

public static class MoradorEndpoints
{
    public static void MapMoradorEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/morador")
            .WithTags("Morador");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var query = new GetAllMoradoresQuery();
            var result = await mediator.Send(query);
            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, mensagem = result.Mensagem });
        })
            .WithName("GetAllMoradores")
            .WithOpenApi()
            .WithSummary("Lista todos os moradores.")
            .WithDescription("Retorna uma lista completa de todos os moradores disponíveis no sistema.")
            .Produces<List<MoradorDto>>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("AdminPolicy")
            .RequireCors()
            .CacheOutput();

        group.MapGet("/paginado", async ([AsParameters] GetAllPagedMoradoresQuery query, ISender sender) =>
        {
            var result = await sender.Send(query);
            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, mensagem = result.Mensagem });
        })
            .WithName("GetAllPagedMoradores")
            .WithOpenApi()
            .WithSummary("Lista os moradores com paginação.")
            .WithDescription("Retorna uma lista paginada de moradores.")
            .Produces<PagedResult<MoradorDto>>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("AdminPolicy")
            .RequireCors()
            .CacheOutput();

        group.MapGet("/{id}", async (int id, IMediator mediator) =>
        {
            var query = new GetMoradorByIdQuery(id);
            var result = await mediator.Send(query);
            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, mensagem = result.Mensagem });
        })
            .WithName("GetMoradorById")
            .WithOpenApi()
            .WithSummary("Retorna um morador por ID.")
            .WithDescription("Retorna um morador por ID disponível no sistema.")
            .Produces<MoradorDto>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("AdminPolicy")
            .RequireCors()
            .CacheOutput();

        group.MapPost("/", async ([FromBody] CreateMoradorCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.Sucesso
                ? Results.CreatedAtRoute("GetMoradorById", new { id = result.Dados.Id }, result.Dados)
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        })
            .WithName("CreateMorador")
            .WithOpenApi()
            .WithSummary("Cadastra um novo morador.")
            .WithDescription("Cadastra um novo morador no sistema.")
            .Produces<MoradorDto>(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("AdminPolicy")
            .RequireCors()
            .CacheOutput();

        group.MapPut("/{id}", async (int id, [FromBody] UpdateMoradorCommand command, ISender sender) =>
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
            .WithName("UpdateMorador")
            .WithOpenApi()
            .WithSummary("Atualiza os dados de um morador existente.")
            .WithDescription("Atualiza um novo morador no sistema.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("AdminPolicy")
            .RequireCors()
            .CacheOutput();

        group.MapDelete("/{id}", async (int id, IMediator mediator) =>
        {
            var command = new DeleteMoradorCommand(id);
            var result = await mediator.Send(command);
            return result.Sucesso
                ? Results.NoContent()
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        })
            .WithName("DeleteMorador")
            .WithOpenApi()
            .WithSummary("Deleta um morador por ID.")
            .WithDescription("Deleta um morador no sistema.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("AdminPolicy")
            .RequireCors()
            .CacheOutput();
    }
}