using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Create;

public record CreateCommandHandlerAuthUser(
    IAuthUserRepository repository,
    IMensageriaService mensageriaService,
    IEmailTemplateService emailTemplateService,
    ILogger<CreateCommandHandlerAuthUser> logger)
        : IRequestHandler<CreateCommandAuthUser, Result<AuthUserDto>>
{
    public async Task<Result<AuthUserDto>> Handle(CreateCommandAuthUser request, CancellationToken cancellationToken)
    {
        string senhaTemporaria = PasswordHasher.GerarSenhaAleatoria(5);
        AuthUser dado = request.ToEntity(senhaTemporaria);
        await repository.CreateAsync(dado, cancellationToken);

        await EnviarEmailBoasVindasAsync(dado, senhaTemporaria);

        return Result<AuthUserDto>.Success(dado.ToDto(), "Usuário criado com sucesso.");
    }

    private async Task EnviarEmailBoasVindasAsync(AuthUser dado, string senhaTemporaria)
    {
        try
        {
            var corpoEmail = emailTemplateService.GerarBoasVindasUsuario(dado.UserName, senhaTemporaria);
            var emailRequest = new EnvioEmailRequest(
                dado.Email,
                "Bem-vindo ao Sistema",
                corpoEmail,
                dado.EmpresaId.GetValueOrDefault()
            );

            await mensageriaService.PublicarMensagemAsync(emailRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Create AuthUser] Falha ao enfileirar e-mail para {Email}", dado.Email);
        }
    }
}