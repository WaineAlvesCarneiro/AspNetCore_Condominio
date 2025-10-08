using AspNetCore_Condominio.Application.Features.Imoveis.Commands.CreateImovel;
using FluentValidation.TestHelper;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.CreateImovel;

public class CreateImovelCommandValidatorTests
{
    private readonly CreateImovelCommandValidator _validator;

    public CreateImovelCommandValidatorTests()
    {
        _validator = new CreateImovelCommandValidator();
    }

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
        var command = GetValidCommand();
        command.Bloco = blocoInvalido;
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.Bloco).WithErrorMessage("Bloco é obrigatório");
    }

    [Fact]
    public void Validator_ApartamentoVazio_DeveTerErro()
    {
        var command = GetValidCommand();
        command.Apartamento = "";
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.Apartamento).WithErrorMessage("Apartamento é obrigatório");
    }

    [Theory]
    [InlineData("12345678901")]
    public void Validator_BoxGaragemMuitoLongo_DeveTerErro(string boxGaragemInvalida)
    {
        var command = GetValidCommand();
        command.BoxGaragem = boxGaragemInvalida;
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.BoxGaragem).WithErrorMessage("O campo Box Garagem precisa ter entre 1 e 10 caracteres");
    }

    [Fact]
    public void Validator_ComandoValido_NaoDeveTerErros()
    {
        var command = GetValidCommand();
        var resultado = _validator.TestValidate(command);
        resultado.ShouldNotHaveAnyValidationErrors();
    }
}
