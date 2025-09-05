using AspNetCore_Condominio.Application.Features.Moradores.Commands.ValidatorBase;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.CreateMorador;

public class CreateMoradorCommandValidator : MoradorCommandValidatorBase<CreateMoradorCommand>
{
    public CreateMoradorCommandValidator()
    {
        ConfigureCommonRules();
    }
}