using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Create;

public record CreateCommandHandlerEmpresa(
    IEmpresaRepository repository,
    IMensageriaService mensageriaService,
    IEmailTemplateService emailTemplateService,
    ILogger<CreateCommandHandlerAuthUser> logger)
        : IRequestHandler<CreateCommandEmpresa, Result<EmpresaDto>>
{
    public async Task<Result<EmpresaDto>> Handle(CreateCommandEmpresa request, CancellationToken cancellationToken)
    {
        Empresa dado = MapearEntidade(request);

        await repository.CreateAsync(dado, cancellationToken);

        EmpresaDto dto = MapearDto(dado);

        var corpoEmail = emailTemplateService.GerarBoasVindasEmpresa(dado.RazaoSocial);
        EnvioEmailRequest emailRequest = PreencherEnvioEmailRequest(dado, corpoEmail);

        try
        {
            await mensageriaService.PublicarEmailFilaAsync(emailRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao publicar e-mail de boas-vindas na fila para a empresa: {dado.RazaoSocial}", dado.Email);
        }

        return Result<EmpresaDto>.Success(dto, "Empresa criada com sucesso e notificação enviada para fila.");
    }

    private static EmpresaDto MapearDto(Empresa dado)
    {
        return new EmpresaDto
        {
            Id = dado.Id,
            RazaoSocial = dado.RazaoSocial,
            Fantasia = dado.Fantasia,
            Cnpj = dado.Cnpj,
            TipoDeCondominio = dado.TipoDeCondominio,
            Nome = dado.Nome,
            Celular = dado.Celular,
            Telefone = dado.Telefone,
            Email = dado.Email,
            Senha = null,
            Host = dado.Host,
            Porta = dado.Porta,
            Cep = dado.Cep,
            Uf = dado.Uf,
            Cidade = dado.Cidade,
            Endereco = dado.Endereco,
            Bairro = dado.Bairro,
            Complemento = dado.Complemento,
            DataInclusao = dado.DataInclusao
        };
    }

    private static Empresa MapearEntidade(CreateCommandEmpresa request)
    {
        var cnpjApenasNumeros = request.Cnpj?.Replace(".", "").Replace("-", "").Replace("/", "");

        string? senhaSmtpCifrada = !string.IsNullOrWhiteSpace(request.Senha)
            ? EncryptionHelper.Encrypt(request.Senha)
            : null;

        return new Empresa
        {
            RazaoSocial = request.RazaoSocial,
            Fantasia = request.Fantasia,
            Cnpj = cnpjApenasNumeros!,
            TipoDeCondominio = request.TipoDeCondominio,
            Nome = request.Nome,
            Celular = request.Celular,
            Telefone = request.Telefone,
            Email = request.Email,
            Senha = senhaSmtpCifrada,
            Host = request.Host,
            Porta = request.Porta,
            Cep = request.Cep,
            Uf = request.Uf,
            Cidade = request.Cidade,
            Endereco = request.Endereco,
            Bairro = request.Bairro,
            Complemento = request.Complemento,
            DataInclusao = DateTime.Now
        };
    }

    private static EnvioEmailRequest PreencherEnvioEmailRequest(Empresa dado, string corpoEmail)
    {
        return new EnvioEmailRequest(
            dado.Email,
            "Bem-vindo - Empresa cadastrada no Sistema",
            corpoEmail,
            dado.Id
        );
    }
}