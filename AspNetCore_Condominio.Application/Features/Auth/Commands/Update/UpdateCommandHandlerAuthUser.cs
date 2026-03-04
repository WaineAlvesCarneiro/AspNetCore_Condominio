using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Update;

public record UpdateCommandHandlerAuthUser(
    IAuthUserRepository repository,
    IMensageriaService mensageriaService,
    IEmailTemplateService emailTemplateService,
    ILogger<CreateCommandHandlerAuthUser> logger)
    : IRequestHandler<UpdateCommandAuthUser, Result<AuthUserDto>>
{
    private readonly IAuthUserRepository _repository = repository;

    public async Task<Result<AuthUserDto>> Handle(UpdateCommandAuthUser request, CancellationToken cancellationToken)
    {
        var dadoToUpdate = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (dadoToUpdate == null)
            return Result<AuthUserDto>.Failure("Usuário não encontrado.");

        MapearEntidade(request, dadoToUpdate);

        await _repository.UpdateAsync(dadoToUpdate, cancellationToken);

        AuthUserDto dto = MapearDto(dadoToUpdate);

        var corpoEmail = emailTemplateService.GerarUsuarioAlterado(dadoToUpdate.UserName);
        EnvioEmailRequest emailRequest = PreencherEnvioEmailRequest(dadoToUpdate, corpoEmail);

        try
        {
            await mensageriaService.PublicarEmailFilaAsync(emailRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao publicar e-mail de alteração na fila para o usuário: {dadoToUpdate.UserName}", dadoToUpdate.Email);
        }

        return Result<AuthUserDto>.Success(dto, "Usuário criado com sucesso.");
    }

    private static void MapearEntidade(UpdateCommandAuthUser request, AuthUser dadoToUpdate)
    {
        dadoToUpdate.Ativo = request.Ativo;
        dadoToUpdate.EmpresaId = request.EmpresaId;
        dadoToUpdate.UserName = request.UserName;
        dadoToUpdate.Email = request.Email;
        if (request.Role.HasValue)
            dadoToUpdate.Role = request.Role.Value;
        dadoToUpdate.DataInclusao = request.DataInclusao;
        dadoToUpdate.DataAlteracao = request.DataAlteracao;
    }

    private static AuthUserDto MapearDto(AuthUser dadoToUpdate)
    {
        return new AuthUserDto
        {
            Id = dadoToUpdate.Id,
            Ativo = dadoToUpdate.Ativo,
            EmpresaAtiva = dadoToUpdate.EmpresaAtiva,
            EmpresaId = dadoToUpdate.EmpresaId,
            UserName = dadoToUpdate.UserName,
            Email = dadoToUpdate.Email,
            PrimeiroAcesso = dadoToUpdate.PrimeiroAcesso,
            Role = dadoToUpdate.Role,
            DataInclusao = dadoToUpdate.DataInclusao,
            DataAlteracao = dadoToUpdate.DataAlteracao,
        };
    }

    private static EnvioEmailRequest PreencherEnvioEmailRequest(AuthUser dado, string corpoEmail)
    {
        return new EnvioEmailRequest(
            dado.Email,
            "Usuário alterado no sistema",
            corpoEmail,
            dado.EmpresaId.GetValueOrDefault()
        );
    }
}