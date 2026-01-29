using AspNetCore_Condominio.Application.Features.Auth.Commands.ValidatorBase;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Create;

public class CreateCommandValidatorAuthUser : CommandValidatorBaseAuthUser<CreateCommandAuthUser>
{
    public CreateCommandValidatorAuthUser()
    {
        ConfigureCommonRules();
    }
}