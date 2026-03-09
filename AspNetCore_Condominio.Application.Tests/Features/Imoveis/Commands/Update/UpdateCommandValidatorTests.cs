using AspNetCore_Condominio.Application.Features.Imoveis.Commands.Update;
using FluentValidation.TestHelper;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.Update;

public class UpdateCommandValidatorTests
{
    private readonly UpdateCommandValidatorImovel _validator = new();

    private UpdateCommandImovel GetValidCommand() => new()
    {
        Id = 1,
        Bloco = "NOVO BL",
        Apartamento = "999",
        BoxGaragem = "ZZZ",
        EmpresaId = 1
    };

    [Fact]
    public void Validator_ComandoValido_DevePassarSemErros()
    {
        // Arrange
        UpdateCommandImovel command = GetValidCommand();
        // Act
        TestValidationResult<UpdateCommandImovel> resultado = _validator.TestValidate(command);
        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_BlocoMuitoCurto_DeveFalharComMensagemCorreta()
    {
        string mensagemEsperada = "Bloco é obrigatório";
        UpdateCommandImovel command = GetValidCommand();
        command.Bloco = "";
        TestValidationResult<UpdateCommandImovel> resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.Bloco).WithErrorMessage(mensagemEsperada);
    }
}
