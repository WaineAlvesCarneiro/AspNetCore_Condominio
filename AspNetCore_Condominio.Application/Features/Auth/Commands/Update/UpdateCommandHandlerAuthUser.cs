using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Update;

public record UpdateCommandHandlerAuthUser(IAuthUserRepository repository)
    : IRequestHandler<UpdateCommandAuthUser, Result<AuthUserDto>>
{
    private readonly IAuthUserRepository _repository = repository;

    public async Task<Result<AuthUserDto>> Handle(UpdateCommandAuthUser request, CancellationToken cancellationToken)
    {
        var dadoToUpdate = await _repository.GetByIdAsync(request.Id);
        if (dadoToUpdate == null)
        {
            return Result<AuthUserDto>.Failure("Usuário não encontrado.");
        }

        dadoToUpdate.EmpresaId = request.EmpresaId;
        dadoToUpdate.UserName = request.UserName;
        dadoToUpdate.PasswordHash = PasswordHasher.HashPassword(request.PasswordHash.ToString());
        dadoToUpdate.Role = request.Role;
        dadoToUpdate.DataInclusao = request.DataInclusao;
        dadoToUpdate.DataAlteracao = request.DataAlteracao;

        await _repository.UpdateAsync(dadoToUpdate);

        var dto = new AuthUserDto
        {
            Id = dadoToUpdate.Id,
            EmpresaId = dadoToUpdate.EmpresaId,
            UserName = dadoToUpdate.UserName,
            PasswordHash = dadoToUpdate.PasswordHash,
            Role = dadoToUpdate.Role,
            DataInclusao = dadoToUpdate.DataInclusao,
            DataAlteracao = dadoToUpdate.DataAlteracao,
        };

        return Result<AuthUserDto>.Success(dto, "Usuário criado com sucesso.");
    }
}