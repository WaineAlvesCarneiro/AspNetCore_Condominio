using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Update;

public record UpdateCommandHandlerEmpresa(
    IEmpresaRepository repository,
    IAuthUserRepository authUserRepository,
    IMensageriaService mensageriaService,
    IEmailTemplateService emailTemplateService,
    ILogger<CreateCommandHandlerAuthUser> logger)
    : IRequestHandler<UpdateCommandEmpresa, Result<EmpresaDto>>
{
    private readonly IEmpresaRepository _repository = repository;
    private readonly IAuthUserRepository _authUserRepository = authUserRepository;

    public async Task<Result<EmpresaDto>> Handle(UpdateCommandEmpresa request, CancellationToken cancellationToken)
    {
        var dadoToUpdate = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (dadoToUpdate == null)
            return Result<EmpresaDto>.Failure("Empresa não encontrada.");
        bool statusMudouParaInativo = MapearEntidade(request, dadoToUpdate);

        await _repository.UpdateAsync(dadoToUpdate, cancellationToken);

        if (statusMudouParaInativo)
            await StatusEmpresaAlterado(request, dadoToUpdate, cancellationToken);

        EmpresaDto dto = MapearDto(dadoToUpdate);

        var corpoEmail = emailTemplateService.GerarEmpresaAlterada(dadoToUpdate.RazaoSocial);
        EnvioEmailRequest emailRequest = PreencherEnvioEmailRequest(dadoToUpdate, corpoEmail);

        try
        {
            await mensageriaService.PublicarEmailFilaAsync(emailRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao publicar e-mail de alteração na fila para o empresa: {dadoToUpdate.RazaoSocial}", dadoToUpdate.Email);
        }

        return Result<EmpresaDto>.Success(dto, "Empresa atualizada com sucesso.");
    }

    private static bool MapearEntidade(UpdateCommandEmpresa request, Empresa dadoToUpdate)
    {
        bool statusMudouParaInativo = dadoToUpdate.Ativo == TipoEmpresaAtivo.Ativo && request.Ativo != TipoEmpresaAtivo.Ativo;

        if (!string.IsNullOrEmpty(request.Senha))
            dadoToUpdate.Senha = EncryptionHelper.Encrypt(request.Senha);

        dadoToUpdate.Ativo = request.Ativo;
        dadoToUpdate.RazaoSocial = request.RazaoSocial;
        dadoToUpdate.Fantasia = request.Fantasia;
        dadoToUpdate.Cnpj = request.Cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
        dadoToUpdate.TipoDeCondominio = request.TipoDeCondominio;
        dadoToUpdate.Nome = request.Nome;
        dadoToUpdate.Celular = request.Celular;
        dadoToUpdate.Telefone = request.Telefone;
        dadoToUpdate.Email = request.Email;
        dadoToUpdate.Host = request.Host;
        dadoToUpdate.Porta = request.Porta;
        dadoToUpdate.Cep = request.Cep;
        dadoToUpdate.Uf = request.Uf;
        dadoToUpdate.Cidade = request.Cidade;
        dadoToUpdate.Endereco = request.Endereco;
        dadoToUpdate.Bairro = request.Bairro;
        dadoToUpdate.Complemento = request.Complemento;
        dadoToUpdate.DataAlteracao = DateTime.Now;
        return statusMudouParaInativo;
    }

    private static EmpresaDto MapearDto(Empresa dadoToUpdate)
    {
        return new EmpresaDto
        {
            Id = dadoToUpdate.Id,
            Ativo = dadoToUpdate.Ativo,
            RazaoSocial = dadoToUpdate.RazaoSocial,
            Fantasia = dadoToUpdate.Fantasia,
            Cnpj = dadoToUpdate.Cnpj,
            TipoDeCondominio = dadoToUpdate.TipoDeCondominio,
            Nome = dadoToUpdate.Nome,
            Celular = dadoToUpdate.Celular,
            Telefone = dadoToUpdate.Telefone ?? "",
            Email = dadoToUpdate.Email,
            Host = dadoToUpdate.Host,
            Porta = dadoToUpdate.Porta,
            Cep = dadoToUpdate.Cep,
            Uf = dadoToUpdate.Uf,
            Cidade = dadoToUpdate.Cidade,
            Endereco = dadoToUpdate.Endereco,
            Bairro = dadoToUpdate.Bairro,
            Complemento = dadoToUpdate.Complemento,
            DataInclusao = dadoToUpdate.DataInclusao,
            DataAlteracao = dadoToUpdate.DataAlteracao
        };
    }

    private static EnvioEmailRequest PreencherEnvioEmailRequest(Empresa dado, string corpoEmail)
    {
        return new EnvioEmailRequest(
            dado.Email,
            "Empresa alterada no sistema",
            corpoEmail,
            dado.Id
        );
    }

    private async Task StatusEmpresaAlterado(UpdateCommandEmpresa request, Empresa dadoToUpdate, CancellationToken cancellationToken)
    {
        var usuarios = await _authUserRepository.GetByEmpresaIdAsync(dadoToUpdate.Id, cancellationToken);

        foreach (var usuario in usuarios)
        {
            usuario.EmpresaAtiva = request.Ativo;
            usuario.DataAlteracao = DateTime.Now;
            await _authUserRepository.UpdateAsync(usuario, cancellationToken);
        }
    }
}