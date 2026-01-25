using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.Create;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.Delete;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.Update;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAll;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetAllPaged;
using AspNetCore_Condominio.Application.Features.Moradores.Queries.GetById;
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
            var query = new GetAllQueryMorador();
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

        group.MapGet("/paginado", async ([AsParameters] GetAllPagedQueryMorador query, ISender sender) =>
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

        group.MapGet("/{id:long}", async (long id, IMediator mediator) =>
        {
            var query = new GetByIdQueryMorador(id);
            var result = await mediator.Send(query);
            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, mensagem = result.Mensagem });
        })
            .WithName("GetByIdMorador")
            .WithOpenApi()
            .WithSummary("Retorna um morador por ID.")
            .WithDescription("Retorna um morador por ID disponível no sistema.")
            .Produces<MoradorDto>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("AdminPolicy")
            .RequireCors()
            .CacheOutput();

        group.MapPost("/", async ([FromBody] CreateCommandMorador command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.Sucesso
                ? Results.CreatedAtRoute("GetByIdMorador", new { id = result.Dados!.Id }, result.Dados)
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

        group.MapPut("/{id:long}", async (long id, [FromBody] UpdateCommandMorador command, ISender sender) =>
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

        group.MapDelete("/{id:long}", async (long id, IMediator mediator) =>
        {
            var command = new DeleteCommandMorador(id);
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