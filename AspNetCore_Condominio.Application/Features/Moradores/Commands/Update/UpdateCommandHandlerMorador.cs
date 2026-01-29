using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.Update;

public class UpdateCommandHandlerMorador(IMoradorRepository repository, IImovelRepository imovelRepository)
    : IRequestHandler<UpdateCommandMorador, Result<MoradorDto>>
{
    private readonly IMoradorRepository _repository = repository;
    private readonly IImovelRepository _imovelRepository = imovelRepository;

    public async Task<Result<MoradorDto>> Handle(UpdateCommandMorador request, CancellationToken cancellationToken)
    {
        var dadoToUpdate = await _repository.GetByIdAsync(request.Id, request.EmpresaId);
        if (dadoToUpdate == null)
        {
            return Result<MoradorDto>.Failure("Morador não encontrado.");
        }

        var imovelExist = await _imovelRepository.GetByIdAsync(request.ImovelId, request.EmpresaId);
        if (imovelExist == null)
        {
            return Result<MoradorDto>.Failure("O imóvel informado não existe.");
        }

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

        await _repository.UpdateAsync(dadoToUpdate);

        var dto = new MoradorDto
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
            ImovelDto = dadoToUpdate.Imovel != null
                ? new ImovelDto
                {
                    Id = dadoToUpdate.Imovel.Id,
                    Bloco = dadoToUpdate.Imovel.Bloco,
                    Apartamento = dadoToUpdate.Imovel.Apartamento,
                    BoxGaragem = dadoToUpdate.Imovel.BoxGaragem,
                    EmpresaId = dadoToUpdate.Imovel.EmpresaId
                }
                : null,
            EmpresaId = dadoToUpdate.EmpresaId,
            EmpresaDto = dadoToUpdate.Empresa != null
                ? new EmpresaDto
                {
                    Id = dadoToUpdate.Id,
                    RazaoSocial = dadoToUpdate.Empresa.RazaoSocial,
                    Fantasia = dadoToUpdate.Empresa.Fantasia,
                    Cnpj = dadoToUpdate.Empresa.Cnpj,
                    TipoDeCondominio = dadoToUpdate.Empresa.TipoDeCondominio,
                    Nome = dadoToUpdate.Nome,
                    Celular = dadoToUpdate.Empresa.Celular,
                    Telefone = dadoToUpdate.Empresa.Telefone!,
                    Email = dadoToUpdate.Empresa.Email,
                    Senha = dadoToUpdate.Empresa.Senha,
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
                }
                : null
        };

        return Result<MoradorDto>.Success(dto, "Morador atualizado com sucesso.");
    }
}