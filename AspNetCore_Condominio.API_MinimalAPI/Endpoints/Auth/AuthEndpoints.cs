using AspNetCore_Condominio.Application.Features.Auth;
using AspNetCore_Condominio.Configurations.ServicesJWT;
using AspNetCore_Condominio.Domain.Entities.Auth;
using MediatR;

namespace AspNetCore_Condominio.API_MinimalAPI.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/Auth/login", async (IMediator mediator, TokenService tokenService, AuthLoginRequest request) =>
        {
            var query = new AuthLoginQuery { Username = request.Username, Password = request.Password };
            var user = await mediator.Send(query);

            if (user == null)
            {
                return Results.Unauthorized();
            }

            var token = tokenService.GenerateToken(user.UserName, user.Role);
            return Results.Ok(new { token });
        })
            .WithSummary("Faça Login para gerar o Token.")
            .WithDescription("Retorna Token válido para o sistema.")
            .AllowAnonymous();
    }
}