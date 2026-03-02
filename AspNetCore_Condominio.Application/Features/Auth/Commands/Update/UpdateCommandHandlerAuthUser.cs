using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Update;

public record UpdateCommandHandlerAuthUser(
    IAuthUserRepository repository,
    IMensageriaService mensageriaService)
    : IRequestHandler<UpdateCommandAuthUser, Result<AuthUserDto>>
{
    private readonly IAuthUserRepository _repository = repository;

    public async Task<Result<AuthUserDto>> Handle(UpdateCommandAuthUser request, CancellationToken cancellationToken)
    {
        var dadoToUpdate = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (dadoToUpdate == null)
        {
            return Result<AuthUserDto>.Failure("Usuário não encontrado.");
        }

        dadoToUpdate.Ativo = request.Ativo;
        dadoToUpdate.EmpresaId = request.EmpresaId;
        dadoToUpdate.UserName = request.UserName;
        dadoToUpdate.Email = request.Email;
        if (request.Role.HasValue)
            dadoToUpdate.Role = request.Role.Value;
        dadoToUpdate.DataInclusao = request.DataInclusao;
        dadoToUpdate.DataAlteracao = request.DataAlteracao;

        await _repository.UpdateAsync(dadoToUpdate, cancellationToken);

        var dto = new AuthUserDto
        {
            Id = dadoToUpdate.Id,
            Ativo = dadoToUpdate.Ativo,
            EmpresaAtiva = dadoToUpdate.EmpresaAtiva,
            EmpresaId = dadoToUpdate.EmpresaId,
            UserName = dadoToUpdate.UserName,
            Email = dadoToUpdate.Email,
            PrimeiroAcesso = dadoToUpdate.PrimeiroAcesso,
            Role = dadoToUpdate.Role,
            DataInclusao = dadoToUpdate.DataInclusao,
            DataAlteracao = dadoToUpdate.DataAlteracao,
        };

        // MENSAGERIA REAL (RabbitMQ)
        // O Handler não espera o e-mail ser enviado, ele apenas publica na fila
        try
        {
            string corpoEmail = $@"
                <h3>Sistema de Condomínio!</h3>
                <p>Usuário alterado com sucesso.</p>";

            await mensageriaService.EnviarEmailAsync(
                dadoToUpdate.Email,
                "Os dados foram alterados",
                corpoEmail,
                dadoToUpdate.EmpresaId
            );
        }
        catch (Exception ex)
        {
            // Importante: Se o RabbitMQ falhar, o usuário já foi alterado no banco.
            // No mercado, usamos padrões como 'Outbox Pattern' para lidar com isso,
            // mas por enquanto, apenas logar o erro é o suficiente.
            Console.WriteLine($"Erro ao publicar no RabbitMQ: {ex.Message}");
        }

        return Result<AuthUserDto>.Success(dto, "Usuário criado com sucesso.");
    }
}