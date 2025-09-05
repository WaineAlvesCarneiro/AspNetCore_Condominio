using AspNetCore_Condominio.Application.Features.Imoveis.Commands.ValidatorBase;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.CreateImovel;

public class CreateImovelCommandValidator : ImovelCommandValidatorBase<CreateImovelCommand>
{
    public CreateImovelCommandValidator()
    {
        ConfigureCommonRules();
    }
}