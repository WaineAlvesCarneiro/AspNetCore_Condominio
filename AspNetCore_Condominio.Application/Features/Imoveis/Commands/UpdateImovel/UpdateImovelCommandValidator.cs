using AspNetCore_Condominio.Application.Features.Imoveis.Commands.ValidatorBase;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.UpdateImovel;

public class UpdateImovelCommandValidator : ImovelCommandValidatorBase<UpdateImovelCommand>
{
    public UpdateImovelCommandValidator()
    {
        ConfigureCommonRules();
    }
}