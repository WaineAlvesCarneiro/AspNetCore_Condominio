using AspNetCore_Condominio.Domain.Enums;
using FluentValidation;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.ValidatorBase;

public abstract class CommandValidatorBaseAuthUser<T> : AbstractValidator<T>
    where T : ICommandBaseAuthUser
{
    protected void ConfigureCommonRules()
    {
        RuleFor(p => p.UserName)
            .NotEmpty().WithMessage("Usuário é obrigatório")
            .Length(3, 100).WithMessage("O campo Usuário precisa ter entre 3 e 100 caracteres");

        RuleFor(p => p.PasswordHash)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .Length(3, 100).WithMessage("O campo Senha precisa ter entre 3 e 100 caracteres");
        
        RuleFor(p => p.Role)
            .Must(x => Enum.IsDefined(typeof(TipoRole), x))
            .WithMessage("Tipo de Perfil inválido. Selecione uma opção válida.");
    }
}