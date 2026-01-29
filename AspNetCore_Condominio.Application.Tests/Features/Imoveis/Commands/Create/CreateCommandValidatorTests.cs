using AspNetCore_Condominio.Application.Features.Imoveis.Commands.Create;
using FluentValidation.TestHelper;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.Create;

public class CreateCommandValidatorTests
{
    private readonly CreateCommandValidatorImovel _validator;

    public CreateCommandValidatorTests()
    {
        _validator = new CreateCommandValidatorImovel();
    }

    private CreateCommandImovel GetValidCommand() => new()
    {
        Bloco = "01",
        Apartamento = "101",
        BoxGaragem = "224",
        EmpresaId = 1
    };

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validator_BlocoVazioOuNulo_DeveTerErro(string blocoInvalido)
    {
        // Arrange
        string messagemEsperada = "Bloco é obrigatório";
        CreateCommandImovel command = GetValidCommand();
        command.Bloco = blocoInvalido;

        // Act
        TestValidationResult<CreateCommandImovel> resultado = _validator.TestValidate(command);

        // Assert
        resultado.ShouldHaveValidationErrorFor(c => c.Bloco).WithErrorMessage(messagemEsperada);
    }

    [Fact]
    public void Validator_ApartamentoVazio_DeveTerErro()
    {
        // Arrange
        string messagemEsperada = "Apartamento é obrigatório";
        CreateCommandImovel command = GetValidCommand();
        command.Apartamento = "";
        // Act
        TestValidationResult<CreateCommandImovel> resultado = _validator.TestValidate(command);
        // Assert
        resultado.ShouldHaveValidationErrorFor(c => c.Apartamento).WithErrorMessage(messagemEsperada);
    }

    [Theory]
    [InlineData("")]
    public void Validator_BoxGaragemMuitoLongo_DeveTerErro(string boxGaragemInvalida)
    {
        // Arrange
        string messagemEsperada = "O campo Garagem precisa ter entre 1 e 100 caracteres";
        CreateCommandImovel command = GetValidCommand();
        command.BoxGaragem = boxGaragemInvalida;
        // Act
        TestValidationResult<CreateCommandImovel> resultado = _validator.TestValidate(command);
        // Assert
        resultado.ShouldHaveValidationErrorFor(c => c.BoxGaragem).WithErrorMessage(messagemEsperada);
    }

    [Fact]
    public void Validator_ComandoValido_NaoDeveTerErros()
    {
        // Arrange
        CreateCommandImovel command = GetValidCommand();
        // Act
        TestValidationResult<CreateCommandImovel> resultado = _validator.TestValidate(command);
        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }
}
