using AspNetCore_Condominio.Application.Features.Imoveis.Commands.ValidatorBase;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.Update;

public class UpdateCommandValidatorImovel : CommandValidatorBaseImovel<UpdateCommandImovel>
{
    public UpdateCommandValidatorImovel()
    {
        ConfigureCommonRules();
    }
}