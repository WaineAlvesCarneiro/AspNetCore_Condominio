using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Mappings;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.Create;

public class CreateCommandHandlerMorador(
    IMoradorRepository repository,
    IImovelRepository imovelRepository,
    IMensageriaService mensageriaService,
    IEmailTemplateService emailTemplateService,
    ILogger<CreateCommandHandlerMorador> logger)
        : IRequestHandler<CreateCommandMorador, Result<MoradorDto>>
{
    public async Task<Result<MoradorDto>> Handle(CreateCommandMorador request, CancellationToken cancellationToken)
    {
        var imovel = await imovelRepository.GetByIdAsync(request.ImovelId, cancellationToken);
        if (imovel == null)
            return Result<MoradorDto>.Failure("O imóvel informado não existe.");

        Morador dado = request.ToEntity();
        await repository.CreateAsync(dado, cancellationToken);

        await EnviarEmailBoasVindasAsync(dado);

        return Result<MoradorDto>.Success(dado.ToDto(), "Morador criado com sucesso.");
    }

    private async Task EnviarEmailBoasVindasAsync(Morador dado)
    {
        try
        {
            var corpoEmail = emailTemplateService.GerarBoasVindasMorador(dado.Nome);
            var emailRequest = new EnvioEmailRequest(
                dado.Email,
                "Bem-vindo ao Sistema!",
                corpoEmail,
                dado.EmpresaId
            );

            await mensageriaService.PublicarMensagemAsync(emailRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Create Morador] Falha ao enfileirar e-mail para {Email}", dado.Email);
        }
    }
}
