using FluentValidation;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.DefinirSenha;

public class DefinirSenhaValidator : AbstractValidator<DefinirSenhaCommand>
{
    public DefinirSenhaValidator()
    {
        RuleFor(p => p.NovaSenha)
            .NotEmpty().WithMessage("A nova senha é obrigatória")
            .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres");
    }
}