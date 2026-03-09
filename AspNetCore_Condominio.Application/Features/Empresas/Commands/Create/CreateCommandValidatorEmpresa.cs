using AspNetCore_Condominio.Application.Features.Empresas.Commands.ValidatorBase;
using AspNetCore_Condominio.Domain.Repositories;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Create;

public class CreateCommandValidatorEmpresa : CommandValidatorBaseEmpresa<CreateCommandEmpresa>
{
    public CreateCommandValidatorEmpresa(IEmpresaRepository repository)
    {
        ConfigureCommonRules(repository);
    }
}