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
        var command = GetValidCommand();
        var resultado = _validator.TestValidate(command);
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_BlocoMuitoCurto_DeveFalharComMensagemCorreta()
    {
        var command = GetValidCommand();
        command.Bloco = "";
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.Bloco).WithErrorMessage("Bloco é obrigatório");
    }
}
