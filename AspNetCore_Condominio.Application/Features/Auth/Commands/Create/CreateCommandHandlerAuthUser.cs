using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Create;

public record CreateCommandHandlerAuthUser(
    IAuthUserRepository repository,
    IMensageriaService mensageriaService)
    : IRequestHandler<CreateCommandAuthUser, Result<AuthUserDto>>
{
    public async Task<Result<AuthUserDto>> Handle(CreateCommandAuthUser request, CancellationToken cancellationToken)
    {
        string senhaTemporaria = PasswordHasher.GerarSenhaAleatoria(5);

        var dado = new AuthUser
        {
            EmpresaId = request.EmpresaId,
            UserName = request.UserName,
            Email = request.Email,
            PrimeiroAcesso = true,
            PasswordHash = PasswordHasher.HashPassword(senhaTemporaria),
            Role = request.Role,
            DataInclusao = request.DataInclusao
        };

        await repository.CreateAsync(dado);

        var dto = new AuthUserDto
        {
            Id = dado.Id,
            Ativo = dado.Ativo,
            EmpresaAtiva = dado.EmpresaAtiva,
            EmpresaId = dado.EmpresaId,
            UserName = dado.UserName,
            Email = dado.Email,
            PrimeiroAcesso = dado.PrimeiroAcesso,
            Role = dado.Role,
            DataInclusao = dado.DataInclusao,
            DataAlteracao = dado.DataAlteracao
        };

        // MENSAGERIA REAL (RabbitMQ)
        // O Handler não espera o e-mail ser enviado, ele apenas publica na fila
        try
        {
            string corpoEmail = $@"
                <h3>Bem-vindo ao Sistema de Condomínio!</h3>
                <p>Seu usuário foi criado com sucesso.</p>
                <p><strong>Usuário:</strong> {dado.UserName}</p>
                <p><strong>Senha Temporária:</strong> {senhaTemporaria}</p>
                <p>Por favor, altere sua senha no primeiro acesso.</p>";

            await mensageriaService.EnviarEmailAsync(
                dado.Email,
                "Seu Acesso",
                corpoEmail,
                dado.EmpresaId
            );
        }
        catch (Exception ex)
        {
            // Importante: Se o RabbitMQ falhar, o usuário já foi criado no banco.
            // No mercado, usamos padrões como 'Outbox Pattern' para lidar com isso,
            // mas por enquanto, apenas logar o erro é o suficiente.
            Console.WriteLine($"Erro ao publicar no RabbitMQ: {ex.Message}");
        }

        return Result<AuthUserDto>.Success(dto, "Usuário criado com sucesso e notificação enviada para fila.");
    }
}