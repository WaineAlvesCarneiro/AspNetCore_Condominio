using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Helpers; // Onde reside o seu EncryptionHelper
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
        // 1. Sanitização de dados (Importante para buscas e indexação)
        var cnpjApenasNumeros = request.Cnpj?.Replace(".", "").Replace("-", "").Replace("/", "");

        // 2. SEGURANÇA: Criptografando a senha do SMTP antes de instanciar a entidade
        // Se a senha não for fornecida, mantemos null, caso contrário, ciframos.
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

            // Aqui entra o dado protegido
            Senha = senhaSmtpCifrada,

            Host = request.Host,
            Porta = request.Porta,
            Cep = request.Cep,
            Uf = request.Uf,
            Cidade = request.Cidade,
            Endereco = request.Endereco,
            Bairro = request.Bairro,
            Complemento = request.Complemento,
            DataInclusao = DateTime.Now // Garantindo que a data seja gerada pelo servidor
        };

        // 3. Persistência
        await repository.CreateAsync(dado);

        // 4. Mapeamento para DTO (Omitindo a senha por segurança)
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

            // NUNCA retorne a senha (nem criptografada) para o Frontend. 
            // O DTO deve ser uma representação segura dos dados.
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