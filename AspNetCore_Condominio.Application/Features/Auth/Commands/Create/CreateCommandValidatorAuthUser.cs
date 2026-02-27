using AspNetCore_Condominio.Application.Features.Auth.Commands.ValidatorBase;
using AspNetCore_Condominio.Domain.Enums;
using FluentValidation;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Create;

public class CreateCommandValidatorAuthUser : CommandValidatorBaseAuthUser<CreateCommandAuthUser>
{
    public CreateCommandValidatorAuthUser()
    {
        ConfigureCommonRules();

        RuleFor(p => p.Role)
            .Must(x => Enum.IsDefined(typeof(TipoRole), x))
            .WithMessage("Tipo de Perfil inválido. Selecione uma opção válida.");
    }
}