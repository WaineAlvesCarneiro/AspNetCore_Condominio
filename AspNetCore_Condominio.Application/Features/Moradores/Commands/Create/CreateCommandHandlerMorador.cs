using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.Create;

public class CreateCommandHandlerMorador(
    IMoradorRepository repository,
    IImovelRepository imovelRepository,
    IMensageriaService mensageriaService)
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
            ImovelId = request.ImovelId,
            EmpresaId = request.EmpresaId
        };

        await _repository.CreateAsync(dado);

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
                }
                : null
        };

        // MENSAGERIA REAL (RabbitMQ)
        // O Handler não espera o e-mail ser enviado, ele apenas publica na fila
        try
        {
            string corpoEmail = $@"
                <h3>Bem-vindo ao Sistema de Condomínio!</h3>
                <p>Morador criado com sucesso.</p>";

            await mensageriaService.EnviarEmailAsync(
                dado.Email,
                "Seu Acesso",
                corpoEmail,
                dado.EmpresaId
            );
        }
        catch (Exception ex)
        {
            // Importante: Se o RabbitMQ falhar, o Morador já foi criado no banco.
            // No mercado, usamos padrões como 'Outbox Pattern' para lidar com isso,
            // mas por enquanto, apenas logar o erro é o suficiente.
            Console.WriteLine($"Erro ao publicar no RabbitMQ: {ex.Message}");
        }

        return Result<MoradorDto>.Success(dto, "Morador criado com sucesso e notificação enviada para fila.");
    }
}
