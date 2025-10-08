using AspNetCore_Condominio.Application.Features.Moradores.Commands.ValidatorBase;
using FluentValidation;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.UpdateMorador;

public class UpdateMoradorCommandValidator : MoradorCommandValidatorBase<UpdateMoradorCommand>
{
    public UpdateMoradorCommandValidator()
    {
        ConfigureCommonRules();

        RuleFor(m => m.Id)
            .GreaterThan(0).WithMessage("O ID do morador deve ser um valor válido.");

        RuleFor(p => p.DataSaida)
            .LessThanOrEqualTo(p => DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Data de saída não pode ser futura");


        RuleFor(p => p.DataSaida)
            .GreaterThan(p => p.DataEntrada)
            .When(p => p.DataSaida.HasValue)
            .WithMessage("Data de saída deve ser maior que a data de entrada");
    }
}