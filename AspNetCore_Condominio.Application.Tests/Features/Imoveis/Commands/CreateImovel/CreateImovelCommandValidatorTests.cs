using AspNetCore_Condominio.Application.Features.Imoveis.Commands.CreateImovel;
using FluentValidation.TestHelper;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.CreateImovel;

public class CreateImovelCommandValidatorTests
{
    private readonly CreateImovelCommandValidator _validator;

    public CreateImovelCommandValidatorTests()
    {
        // Inicializa o validador que será testado.
        // Isso permite que os testes verifiquem se o validador está funcionando corretamente.
        _validator = new CreateImovelCommandValidator();
    }

    // Método auxiliar para criar um comando válido.
    private CreateImovelCommand GetValidCommand() => new()
    {
        Bloco = "01",
        Apartamento = "101",
        BoxGaragem = "224"
    };

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validator_BlocoVazioOuNulo_DeveTerErro(string blocoInvalido)
    {
        // Arrange
        string messagemEsperada = "Bloco é obrigatório";
        CreateImovelCommand command = GetValidCommand();
        command.Bloco = blocoInvalido;

        // Act
        TestValidationResult<CreateImovelCommand> resultado = _validator.TestValidate(command);

        // Assert
        // Verifica se o erro de validação esperado ocorreu.
        // O campo "Bloco" é obrigatório, então deve haver um erro se estiver vazio ou nulo.
        // A mensagem de erro esperada é "Bloco é obrigatório".
        // ShouldHaveValidationErrorFor verifica se há um erro de validação para o campo especificado.
        // WithErrorMessage verifica se a mensagem de erro corresponde à esperada.
        resultado.ShouldHaveValidationErrorFor(c => c.Bloco).WithErrorMessage(messagemEsperada);
    }

    [Fact]
    public void Validator_ApartamentoVazio_DeveTerErro()
    {
        // Arrange
        string messagemEsperada = "Apartamento é obrigatório";
        CreateImovelCommand command = GetValidCommand();
        command.Apartamento = "";

        // Act
        TestValidationResult<CreateImovelCommand> resultado = _validator.TestValidate(command);

        // Assert
        resultado.ShouldHaveValidationErrorFor(c => c.Apartamento).WithErrorMessage(messagemEsperada);
    }

    [Theory]
    [InlineData("12345678901")]
    public void Validator_BoxGaragemMuitoLongo_DeveTerErro(string boxGaragemInvalida)
    {
        string messagemEsperada = "BoxGaragem deve ter no máximo 10 caracteres";
        CreateImovelCommand command = GetValidCommand();
        command.BoxGaragem = boxGaragemInvalida;
        TestValidationResult<CreateImovelCommand> resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.BoxGaragem).WithErrorMessage(messagemEsperada);
    }

    [Fact]
    public void Validator_ComandoValido_NaoDeveTerErros()
    {
        CreateImovelCommand command = GetValidCommand();
        TestValidationResult<CreateImovelCommand> resultado = _validator.TestValidate(command);
        resultado.ShouldNotHaveAnyValidationErrors();
    }
}
