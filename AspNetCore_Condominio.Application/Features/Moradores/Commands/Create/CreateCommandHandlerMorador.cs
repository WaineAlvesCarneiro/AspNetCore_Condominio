using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
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
    ILogger<CreateCommandHandlerAuthUser> logger)
    : IRequestHandler<CreateCommandMorador, Result<MoradorDto>>
{
    private readonly IMoradorRepository _repository = repository;
    private readonly IImovelRepository _imovelRepository = imovelRepository;

    public async Task<Result<MoradorDto>> Handle(CreateCommandMorador request, CancellationToken cancellationToken)
    {
        var imovelExist = await _imovelRepository.GetByIdAsync(request.ImovelId, cancellationToken);

        if (imovelExist == null)
            return Result<MoradorDto>.Failure("O imóvel informado não existe.");

        Morador dado = MapearEntidade(request);

        await _repository.CreateAsync(dado, cancellationToken);

        MoradorDto dto = MapearDto(dado);

        var corpoEmail = emailTemplateService.GerarBoasVindasMorador(dado.Nome);
        EnvioEmailRequest emailRequest = PreencherEnvioEmailRequest(dado, corpoEmail);

        try
        {
            await mensageriaService.PublicarEmailFilaAsync(emailRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao publicar e-mail de boas-vindas na fila para o morador: {dado.Nome}", dado.Email);
        }

        return Result<MoradorDto>.Success(dto, "Morador criado com sucesso e notificação enviada para fila.");
    }

    private static Morador MapearEntidade(CreateCommandMorador request)
    {
        return new Morador
        {
            Nome = request.Nome,
            Celular = request.Celular,
            Email = request.Email,
            IsProprietario = request.IsProprietario,
            DataEntrada = request.DataEntrada,
            DataSaida = null,
            DataInclusao = request.DataInclusao,
            DataAlteracao = null,
            ImovelId = request.ImovelId,
            EmpresaId = request.EmpresaId
        };
    }

    private static MoradorDto MapearDto(Morador dado)
    {
        return new MoradorDto
        {
            Id = dado.Id,
            Nome = dado.Nome,
            Celular = dado.Celular,
            Email = dado.Email,
            IsProprietario = dado.IsProprietario,
            DataEntrada = dado.DataEntrada,
            DataSaida = dado.DataSaida,
            DataInclusao = dado.DataInclusao,
            DataAlteracao = dado.DataAlteracao,
            ImovelId = dado.ImovelId,
            ImovelDto = dado.Imovel != null ? MapearEntidadeImovel(dado) : null,
            EmpresaId = dado.EmpresaId,
            EmpresaDto = dado.Empresa != null ? MapearEntidadeEmpresa(dado) : null
        };
    }

    private static ImovelDto MapearEntidadeImovel(Morador dado)
    {
        return new ImovelDto
        {
            Id = dado.Imovel!.Id,
            Bloco = dado.Imovel.Bloco,
            Apartamento = dado.Imovel.Apartamento,
            BoxGaragem = dado.Imovel.BoxGaragem,
            EmpresaId = dado.Imovel.EmpresaId
        };
    }

    private static EmpresaDto MapearEntidadeEmpresa(Morador dado)
    {
        return new EmpresaDto
        {
            Id = dado.Id,
            RazaoSocial = dado.Empresa!.RazaoSocial,
            Fantasia = dado.Empresa.Fantasia,
            Cnpj = dado.Empresa.Cnpj,
            TipoDeCondominio = dado.Empresa.TipoDeCondominio,
            Nome = dado.Nome,
            Celular = dado.Empresa.Celular,
            Telefone = dado.Empresa.Telefone!,
            Email = dado.Empresa.Email,
            Senha = null,
            Host = dado.Empresa.Host,
            Porta = dado.Empresa.Porta,
            Cep = dado.Empresa.Cep,
            Uf = dado.Empresa.Uf,
            Cidade = dado.Empresa.Cidade,
            Endereco = dado.Empresa.Endereco,
            Bairro = dado.Empresa.Bairro,
            Complemento = dado.Empresa.Complemento,
            DataInclusao = dado.Empresa.DataInclusao,
            DataAlteracao = dado.Empresa.DataAlteracao
        };
    }

    private static EnvioEmailRequest PreencherEnvioEmailRequest(Morador dado, string corpoEmail)
    {
        return new EnvioEmailRequest(
            dado.Email,
            "Bem-vindo - Seu Acesso ao Sistema",
            corpoEmail,
            dado.EmpresaId.GetHashCode()
        );
    }
}
