using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Create;

public record CreateCommandHandlerEmpresa(
    IEmpresaRepository repository,
    IMensageriaService mensageriaService)
        : IRequestHandler<CreateCommandEmpresa, Result<EmpresaDto>>
{
    public async Task<Result<EmpresaDto>> Handle(CreateCommandEmpresa request, CancellationToken cancellationToken)
    {
        var cnpjApenasNumeros = request.Cnpj?.Replace(".", "").Replace("-", "").Replace("/", "");

        string? senhaSmtpCifrada = !string.IsNullOrWhiteSpace(request.Senha)
            ? EncryptionHelper.Encrypt(request.Senha)
            : null;

        var dado = new Empresa
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

        await repository.CreateAsync(dado, cancellationToken);

        var dto = new EmpresaDto
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

        // MENSAGERIA REAL (RabbitMQ)
        // O Handler não espera o e-mail ser enviado, ele apenas publica na fila
        try
        {
            string corpoEmail = $@"
                <h3>Bem-vindo ao Sistema de Condomínio!</h3>
                <p>Seu condomínio foi cadastrado com sucesso.</p>";

            await mensageriaService.EnviarEmailAsync(
                dado.Email,
                "Seu Acesso",
                corpoEmail,
                dado.Id
            );
        }
        catch (Exception ex)
        {
            // Importante: Se o RabbitMQ falhar, a empresa já foi criada no banco.
            // No mercado, usamos padrões como 'Outbox Pattern' para lidar com isso,
            // mas por enquanto, apenas logar o erro é o suficiente.
            Console.WriteLine($"Erro ao publicar no RabbitMQ: {ex.Message}");
        }

        return Result<EmpresaDto>.Success(dto, "Empresa criada com sucesso e notificação enviada para fila.");
    }
}