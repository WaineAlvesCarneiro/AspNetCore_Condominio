using AspNetCore_Condominio.Application.Features.Moradores.Commands.ValidatorBase;

namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.Create;

public class CreateCommandValidatorMorador : CommandValidatorBaseMorador<CreateCommandMorador>
{
    public CreateCommandValidatorMorador()
    {
        ConfigureCommonRules();
    }
}