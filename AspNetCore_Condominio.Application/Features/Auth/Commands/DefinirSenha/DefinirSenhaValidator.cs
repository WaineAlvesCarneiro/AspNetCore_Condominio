using FluentValidation;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.DefinirSenha;

public class DefinirSenhaValidator : AbstractValidator<DefinirSenhaCommand>
{
    public DefinirSenhaValidator()
    {
        RuleFor(p => p.NovaSenha)
            .NotEmpty().WithMessage("A nova senha é obrigatória")
            .MinimumLength(5).WithMessage("A senha deve ter no mínimo 5 caracteres");
    }
}