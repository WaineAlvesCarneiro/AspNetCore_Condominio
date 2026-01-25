using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Events;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.Create;

public class CreateCommandHandlerMorador(IMoradorRepository repository, IImovelRepository imovelRepository, IMediator mediator)
    : IRequestHandler<CreateCommandMorador, Result<MoradorDto>>
{
    private readonly IMoradorRepository _repository = repository;
    private readonly IImovelRepository _imovelRepository = imovelRepository;

    public async Task<Result<MoradorDto>> Handle(CreateCommandMorador request, CancellationToken cancellationToken)
    {
        var imovelExist = await _imovelRepository.GetByIdAsync(request.ImovelId);
        if (imovelExist == null)
        {
            return Result<MoradorDto>.Failure("O imóvel informado não existe.");
        }

        var dado = new Morador
        {
            Nome = request.Nome,
            Celular = request.Celular,
            Email = request.Email,
            IsProprietario = request.IsProprietario,
            DataEntrada = request.DataEntrada,
            DataSaida = null,
            DataInclusao = request.DataInclusao,
            DataAlteracao = null,
            ImovelId = request.ImovelId
        };

        await _repository.CreateAsync(dado);

        await mediator.Publish(new CriadoEventEmail<Morador>(dado, true), cancellationToken);

        var dto = new MoradorDto
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
            ImovelDto = dado.Imovel != null
                ? new ImovelDto
                {
                    Id = dado.Imovel.Id,
                    Bloco = dado.Imovel.Bloco,
                    Apartamento = dado.Imovel.Apartamento,
                    BoxGaragem = dado.Imovel.BoxGaragem,
                    EmpresaId = dado.Imovel.EmpresaId
                }
                : null,
            EmpresaId = dado.EmpresaId,
            EmpresaDto = dado.Empresa != null
                ? new EmpresaDto
                {
                    Id = dado.Id,
                    RazaoSocial = dado.Empresa.RazaoSocial,
                    Fantasia = dado.Empresa.Fantasia,
                    Cnpj = dado.Empresa.Cnpj,
                    TipoDeCondominio = dado.Empresa.TipoDeCondominio,
                    Nome = dado.Nome,
                    Celular = dado.Empresa.Celular,
                    Telefone = dado.Empresa.Telefone!,
                    Email = dado.Empresa.Email,
                    Cep = dado.Empresa.Cep,
                    Uf = dado.Empresa.Uf,
                    Cidade = dado.Empresa.Cidade,
                    Endereco = dado.Empresa.Endereco,
                    Complemento = dado.Empresa.Complemento,
                    DataInclusao = dado.Empresa.DataInclusao,
                    DataAlteracao = dado.Empresa.DataAlteracao
                }
                : null
        };

        return Result<MoradorDto>.Success(dto, "Morador criado com sucesso.");
    }
}
