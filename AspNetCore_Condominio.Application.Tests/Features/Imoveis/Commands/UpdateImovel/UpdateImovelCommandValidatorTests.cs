using AspNetCore_Condominio.Application.Features.Imoveis.Commands.UpdateImovel;
using FluentValidation.TestHelper;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.UpdateImovel;

public class UpdateImovelCommandValidatorTests
{
    private readonly UpdateImovelCommandValidator _validator = new();

    private UpdateImovelCommand GetValidCommand() => new()
    {
        Id = 1,
        Bloco = "NOVO BL",
        Apartamento = "999",
        BoxGaragem = "ZZZ"
    };

    [Fact]
    public void Validator_ComandoValido_DevePassarSemErros()
    {
        // Arrange
        UpdateImovelCommand command = GetValidCommand();
        // Act
        TestValidationResult<UpdateImovelCommand> resultado = _validator.TestValidate(command);
        // Assert
        // ShouldNotHaveAnyValidationErrors verifica se não há erros de validação no resultado da validação.
        // Se houver algum erro, o teste falhará.
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_BlocoMuitoCurto_DeveFalharComMensagemCorreta()
    {
        string mensagemEsperada = "Bloco é obrigatório";
        UpdateImovelCommand command = GetValidCommand();
        command.Bloco = "";
        TestValidationResult<UpdateImovelCommand> resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.Bloco).WithErrorMessage(mensagemEsperada);
    }
}
