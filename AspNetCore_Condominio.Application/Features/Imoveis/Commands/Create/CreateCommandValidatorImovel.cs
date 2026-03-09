using AspNetCore_Condominio.Application.Features.Imoveis.Commands.ValidatorBase;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.Create;

public class CreateCommandValidatorImovel : CommandValidatorBaseImovel<CreateCommandImovel>
{
    public CreateCommandValidatorImovel()
    {
        ConfigureCommonRules();
    }
}