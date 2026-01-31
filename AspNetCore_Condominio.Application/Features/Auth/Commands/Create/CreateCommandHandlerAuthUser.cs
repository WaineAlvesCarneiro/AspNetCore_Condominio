using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Create;

public record CreateCommandHandlerAuthUser(IAuthUserRepository repository)
    : IRequestHandler<CreateCommandAuthUser, Result<AuthUserDto>>
{
    public async Task<Result<AuthUserDto>> Handle(CreateCommandAuthUser request, CancellationToken cancellationToken)
    {
        string senhaTemporaria = PasswordHasher.GerarSenhaAleatoria(8);

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
            EmpresaId = dado.EmpresaId,
            UserName = dado.UserName,
            Email = dado.Email,
            PrimeiroAcesso = dado.PrimeiroAcesso,
            Role = dado.Role,
            DataInclusao = dado.DataInclusao,
            DataAlteracao = dado.DataAlteracao
        };

        // 4. MENSAGERIA (Simulação)
        // TODO: Publicar evento no Bus: new UserCreatedEvent(novoUsuario.Email, senhaTemporaria)
        Console.WriteLine("=================================================");
        Console.WriteLine($"Simulação de E-mail enviado para: {dado.Email}");
        Console.WriteLine($"Senha Temporária Gerada: {senhaTemporaria}");
        Console.WriteLine("=================================================");

        return Result<AuthUserDto>.Success(dto, "Usuário criado com sucesso.");
    }
}