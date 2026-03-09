using AspNetCore_Condominio.Application.Features.Empresas.Commands.ValidatorBase;
using AspNetCore_Condominio.Domain.Repositories;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Update;

public class UpdateCommandValidatorEmpresa : CommandValidatorBaseEmpresa<UpdateCommandEmpresa>
{
    public UpdateCommandValidatorEmpresa(IEmpresaRepository repository)
    {
        ConfigureCommonRules(repository);
    }
}