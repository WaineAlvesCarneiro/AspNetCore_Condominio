using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.Update;

public class UpdateCommandHandlerMorador(
    IMoradorRepository repository,
    IImovelRepository imovelRepository,
    IMensageriaService mensageriaService,
    IEmailTemplateService emailTemplateService,
    ILogger<CreateCommandHandlerAuthUser> logger)
    : IRequestHandler<UpdateCommandMorador, Result<MoradorDto>>
{
    private readonly IMoradorRepository _repository = repository;
    private readonly IImovelRepository _imovelRepository = imovelRepository;

    public async Task<Result<MoradorDto>> Handle(UpdateCommandMorador request, CancellationToken cancellationToken)
    {
        var dadoToUpdate = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (dadoToUpdate == null)
            return Result<MoradorDto>.Failure("Morador não encontrado.");

        var imovelExist = await _imovelRepository.GetByIdAsync(request.ImovelId, cancellationToken);

        if (imovelExist == null)
            return Result<MoradorDto>.Failure("O imóvel informado não existe.");

        MapearEntidade(request, dadoToUpdate);

        await _repository.UpdateAsync(dadoToUpdate, cancellationToken);
        
        MoradorDto dto = MapearDto(dadoToUpdate);

        var corpoEmail = emailTemplateService.GerarMoradorAlterado(dadoToUpdate.Nome);
        EnvioEmailRequest emailRequest = PreencherEnvioEmailRequest(dadoToUpdate, corpoEmail);

        try
        {
            await mensageriaService.PublicarEmailFilaAsync(emailRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao publicar e-mail de alteração na fila para o morador: {dadoToUpdate.Nome}", dadoToUpdate.Email);
        }

        return Result<MoradorDto>.Success(dto, "Morador atualizado com sucesso.");
    }

    private static void MapearEntidade(UpdateCommandMorador request, Morador dadoToUpdate)
    {
        dadoToUpdate.Nome = request.Nome;
        dadoToUpdate.Celular = request.Celular;
        dadoToUpdate.Email = request.Email;
        dadoToUpdate.IsProprietario = request.IsProprietario;
        dadoToUpdate.DataEntrada = request.DataEntrada;
        dadoToUpdate.DataSaida = request.DataSaida!;
        dadoToUpdate.ImovelId = request.ImovelId;
        dadoToUpdate.DataAlteracao = request.DataAlteracao!;
        dadoToUpdate.DataInclusao = dadoToUpdate.DataInclusao;
        dadoToUpdate.EmpresaId = dadoToUpdate.EmpresaId;
    }

    private static MoradorDto MapearDto(Morador dadoToUpdate)
    {
        return new MoradorDto
        {
            Id = dadoToUpdate.Id,
            Nome = dadoToUpdate.Nome,
            Celular = dadoToUpdate.Celular,
            Email = dadoToUpdate.Email,
            IsProprietario = dadoToUpdate.IsProprietario,
            DataEntrada = dadoToUpdate.DataEntrada,
            DataSaida = dadoToUpdate.DataSaida,
            DataInclusao = dadoToUpdate.DataInclusao,
            DataAlteracao = dadoToUpdate.DataAlteracao,
            ImovelId = dadoToUpdate.ImovelId,
            ImovelDto = dadoToUpdate.Imovel != null ? MapearEntidadeImovel(dadoToUpdate) : null,
            EmpresaId = dadoToUpdate.EmpresaId,
            EmpresaDto = dadoToUpdate.Empresa != null ? MapearEntidadeEmpresa(dadoToUpdate) : null
        };
    }

    private static ImovelDto MapearEntidadeImovel(Morador dadoToUpdate)
    {
        return new ImovelDto
        {
            Id = dadoToUpdate.Imovel!.Id,
            Bloco = dadoToUpdate.Imovel.Bloco,
            Apartamento = dadoToUpdate.Imovel.Apartamento,
            BoxGaragem = dadoToUpdate.Imovel.BoxGaragem,
            EmpresaId = dadoToUpdate.Imovel.EmpresaId
        };
    }

    private static EmpresaDto MapearEntidadeEmpresa(Morador dadoToUpdate)
    {
        return new EmpresaDto
        {
            Id = dadoToUpdate.Id,
            RazaoSocial = dadoToUpdate.Empresa!.RazaoSocial,
            Fantasia = dadoToUpdate.Empresa.Fantasia,
            Cnpj = dadoToUpdate.Empresa.Cnpj,
            TipoDeCondominio = dadoToUpdate.Empresa.TipoDeCondominio,
            Nome = dadoToUpdate.Nome,
            Celular = dadoToUpdate.Empresa.Celular,
            Telefone = dadoToUpdate.Empresa.Telefone!,
            Email = dadoToUpdate.Empresa.Email,
            Senha = null,
            Host = dadoToUpdate.Empresa.Host,
            Porta = dadoToUpdate.Empresa.Porta,
            Cep = dadoToUpdate.Empresa.Cep,
            Uf = dadoToUpdate.Empresa.Uf,
            Cidade = dadoToUpdate.Empresa.Cidade,
            Endereco = dadoToUpdate.Empresa.Endereco,
            Bairro = dadoToUpdate.Empresa.Bairro,
            Complemento = dadoToUpdate.Empresa.Complemento,
            DataInclusao = dadoToUpdate.Empresa.DataInclusao,
            DataAlteracao = dadoToUpdate.Empresa.DataAlteracao
        };
    }

    private static EnvioEmailRequest PreencherEnvioEmailRequest(Morador dado, string corpoEmail)
    {
        return new EnvioEmailRequest(
            dado.Email,
            "Morador alterado no sistema",
            corpoEmail,
            dado.Id
        );
    }
}