using AspNetCore_Condominio.Application.Features.Empresas.Commands.ValidatorBase;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Create;

public class CreateCommandValidatorEmpresa : CommandValidatorBaseEmpresa<CreateCommandEmpresa>
{
    public CreateCommandValidatorEmpresa()
    {
        ConfigureCommonRules();
    }
}