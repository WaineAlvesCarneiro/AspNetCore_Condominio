using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
using AspNetCore_Condominio.Domain.Enums;
using FluentValidation.TestHelper;

namespace AspNetCore_Condominio.Application.Tests.Features.Auth.Commands.Create;

public class CreateCommandValidatorTests
{
    private readonly CreateCommandValidatorAuthUser _validator;

    public CreateCommandValidatorTests()
    {
        _validator = new CreateCommandValidatorAuthUser();
    }

    private CreateCommandAuthUser GetValidCommand() => new()
    {
        EmpresaId = 1,
        UserName = "Admin",
        PasswordHash = "12345",
        Role = (TipoRole)1,
        DataInclusao = DateTime.UtcNow
    };

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validator_UserName_Vazio_Ou_Nulo_Deve_Ter_Erro(string userNameInvalida)
    {
        // Arrange
        string messagemEsperada = "Usuário é obrigatório";
        CreateCommandAuthUser command = GetValidCommand();
        command.UserName = userNameInvalida;

        // Act
        TestValidationResult<CreateCommandAuthUser> resultado = _validator.TestValidate(command);

        // Assert
        resultado.ShouldHaveValidationErrorFor(c => c.UserName).WithErrorMessage(messagemEsperada);
    }

    [Fact]
    public void Validator_UserName_Vazio_Deve_Ter_Erro()
    {
        // Arrange
        string messagemEsperada = "Usuário é obrigatório";
        CreateCommandAuthUser command = GetValidCommand();
        command.UserName = "";
        // Act
        TestValidationResult<CreateCommandAuthUser> resultado = _validator.TestValidate(command);
        // Assert
        resultado.ShouldHaveValidationErrorFor(c => c.UserName).WithErrorMessage(messagemEsperada);
    }

    [Fact]
    public void Validator_Comando_Valido_Nao_Deve_Ter_Erros()
    {
        // Arrange
        CreateCommandAuthUser command = GetValidCommand();
        // Act
        TestValidationResult<CreateCommandAuthUser> resultado = _validator.TestValidate(command);
        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }
}
