using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Update;

public class UpdateCommandHandlerEmpresa(
    IEmpresaRepository repository,
    IAuthUserRepository authUserRepository,
    IMensageriaService mensageriaService,
    IEmailTemplateService emailTemplateService,
    ILogger<UpdateCommandHandlerEmpresa> logger)
        : IRequestHandler<UpdateCommandEmpresa, Result<EmpresaDto>>
{
    public async Task<Result<EmpresaDto>> Handle(UpdateCommandEmpresa request, CancellationToken cancellationToken)
    {
        var dadoToUpdate = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (dadoToUpdate == null)
            return Result<EmpresaDto>.Failure("Empresa não encontrada.");

        bool statusMudouParaInativo = dadoToUpdate.UpdateFromCommand(request);
        await repository.UpdateAsync(dadoToUpdate, cancellationToken);

        if (statusMudouParaInativo)
            await SincronizarStatusUsuariosAsync(dadoToUpdate.Id, request.Ativo, cancellationToken);

        await EnviarEmailAlteracoesAsync(dadoToUpdate);

        return Result<EmpresaDto>.Success(dadoToUpdate.ToDto(), "Empresa atualizada com sucesso.");
    }

    private async Task EnviarEmailAlteracoesAsync(Empresa dadoToUpdate)
    {
        try
        {
            var corpoEmail = emailTemplateService.GerarEmpresaAlterada(dadoToUpdate.RazaoSocial);
            var emailRequest = new EnvioEmailRequest(
                dadoToUpdate.Email,
                "Empresa - Alteração de Dados Cadastrais",
                corpoEmail,
                dadoToUpdate.Id
            );

            await mensageriaService.PublicarMensagemAsync(emailRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Update Empresa] Falha ao enfileirar e-mail para {Email}", dadoToUpdate.Email);
        }
    }

    private async Task SincronizarStatusUsuariosAsync(long empresaId, TipoEmpresaAtivo novoStatus, CancellationToken cancellationToken)
    {
        var usuarios = await authUserRepository.GetByEmpresaIdAsync(empresaId, cancellationToken);

        foreach (var usuario in usuarios)
        {
            usuario.EmpresaAtiva = novoStatus;
            usuario.DataAlteracao = DateTime.Now;
            await authUserRepository.UpdateAsync(usuario, cancellationToken);
        }

        logger.LogInformation("[Update Status Usuarios] Status de {Qtd} usuários sincronizados com a empresa {Id}", usuarios.Count(), empresaId);
    }
}