using AspNetCore_Condominio.Application.Features.Auth.Commands.Update;
using AspNetCore_Condominio.Domain.Enums;
using FluentValidation.TestHelper;

namespace AspNetCore_Condominio.Application.Tests.Features.Auth.Commands.Update;

public class UpdateCommandValidatorTests
{
    private readonly UpdateCommandValidatorAuthUser _validator = new();

    private UpdateCommandAuthUser GetValidCommand() => new()
    {
        Id = Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B"),
        EmpresaId = 1,
        UserName = "Admin",
        PasswordHash = "12345",
        Role = (TipoRole)1,
        DataInclusao = DateTime.UtcNow
    };

    [Fact]
    public void Validator_ComandoValido_DevePassarSemErros()
    {
        // Arrange
        UpdateCommandAuthUser command = GetValidCommand();
        // Act
        TestValidationResult<UpdateCommandAuthUser> resultado = _validator.TestValidate(command);
        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_UserNameMuitoCurto_DeveFalharComMensagemCorreta()
    {
        string mensagemEsperada = "Usuário é obrigatório";
        UpdateCommandAuthUser command = GetValidCommand();
        command.UserName = "";
        TestValidationResult<UpdateCommandAuthUser> resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.UserName).WithErrorMessage(mensagemEsperada);
    }
}
