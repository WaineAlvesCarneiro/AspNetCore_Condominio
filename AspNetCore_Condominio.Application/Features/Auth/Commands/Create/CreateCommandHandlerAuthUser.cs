using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Helpers;
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

        AuthUser dado = MapearEntidade(request, senhaTemporaria);

        await repository.CreateAsync(dado, cancellationToken);

        AuthUserDto dto = MapearDto(dado);

        var corpoEmail = emailTemplateService.GerarBoasVindasUsuario(dado.UserName, senhaTemporaria);
        EnvioEmailRequest emailRequest = PreencherEnvioEmailRequest(dado, corpoEmail);

        try
        {
            await mensageriaService.PublicarEmailFilaAsync(emailRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao publicar e-mail de boas-vindas na fila para o usuário: {dado.UserName}", dado.Email);
        }

        return Result<AuthUserDto>.Success(dto, "Usuário criado com sucesso e notificação enviada para fila.");
    }

    private static AuthUser MapearEntidade(CreateCommandAuthUser request, string senhaTemporaria)
    {
        return new AuthUser
        {
            EmpresaId = request.EmpresaId,
            UserName = request.UserName,
            Email = request.Email,
            PrimeiroAcesso = true,
            PasswordHash = PasswordHasher.HashPassword(senhaTemporaria),
            Role = request.Role,
            DataInclusao = request.DataInclusao
        };
    }

    private static AuthUserDto MapearDto(AuthUser dado)
    {
        return new AuthUserDto
        {
            Id = dado.Id,
            Ativo = dado.Ativo,
            EmpresaAtiva = dado.EmpresaAtiva,
            EmpresaId = dado.EmpresaId,
            UserName = dado.UserName,
            Email = dado.Email,
            PrimeiroAcesso = dado.PrimeiroAcesso,
            Role = dado.Role,
            DataInclusao = dado.DataInclusao,
            DataAlteracao = dado.DataAlteracao
        };
    }

    private static EnvioEmailRequest PreencherEnvioEmailRequest(AuthUser dado, string corpoEmail)
    {
        return new EnvioEmailRequest(
            dado.Email,
            "Bem-vindo - Seu Acesso ao Sistema",
            corpoEmail,
            dado.EmpresaId.GetValueOrDefault()
        );
    }
}