using AspNetCore_Condominio.Application.Features.Auth.Commands.ValidatorBase;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.Update;

public class UpdateCommandValidatorAuthUser : CommandValidatorBaseAuthUser<UpdateCommandAuthUser>
{
    public UpdateCommandValidatorAuthUser()
    {
        ConfigureCommonRules();
    }
}