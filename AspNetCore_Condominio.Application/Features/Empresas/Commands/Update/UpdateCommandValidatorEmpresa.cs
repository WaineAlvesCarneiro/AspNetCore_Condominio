using AspNetCore_Condominio.Application.Features.Empresas.Commands.ValidatorBase;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Update;

public class UpdateCommandValidatorEmpresa : CommandValidatorBaseEmpresa<UpdateCommandEmpresa>
{
    public UpdateCommandValidatorEmpresa()
    {
        ConfigureCommonRules();
    }
}