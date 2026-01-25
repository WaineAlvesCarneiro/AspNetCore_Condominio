using AspNetCore_Condominio.Application.Features.Moradores.Commands.ValidatorBase;
using FluentValidation;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.Update;

public class UpdateCommandValidatorMorador : CommandValidatorBaseMorador<UpdateCommandMorador>
{
    public UpdateCommandValidatorMorador()
    {
        ConfigureCommonRules();

        RuleFor(m => m.Id)
            .GreaterThan(0).WithMessage("O ID do morador deve ser um valor válido.");

        RuleFor(p => p.DataSaida)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Data de saída não pode ser futura");

        RuleFor(p => p.DataSaida)
            .GreaterThanOrEqualTo(p => p.DataEntrada)
            .When(p => p.DataSaida.HasValue)
            .WithMessage("Data de saída deve ser maior ou igual à data de entrada");
    }
}