using AspNetCore_Condominio.Application.Features.Moradores.Commands.UpdateMorador;
using FluentValidation.TestHelper;
using DateOnly = System.DateOnly;

namespace AspNetCore_Condominio.Application.Tests.Features.Moradores.Commands.UpdateMorador;

public class UpdateMoradorCommandValidatorTests
{
    private readonly UpdateMoradorCommandValidator _validator = new();
    private readonly DateOnly _dataEntradaValida = new DateOnly(2023, 1, 10);

    private UpdateMoradorCommand GetValidCommand() => new()
    {
        Id = 1,
        Nome = "Novo Nome",
        Celular = "11998765432",
        Email = "novo.email@cond.com",
        IsProprietario = true,
        DataEntrada = _dataEntradaValida,
        DataInclusao = new DateOnly(2023, 1, 10),
        DataSaida = null,
        DataAlteracao = DateOnly.FromDateTime(DateTime.Today),
        ImovelId = 1
    };

    [Fact]
    public void Validator_ComandoValido_NaoDeveTerErros()
    {
        var command = GetValidCommand();
        var resultado = _validator.TestValidate(command);
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_IdInvalido_DeveTerErro()
    {
        var command = GetValidCommand();
        command.Id = 0;
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.Id).WithErrorMessage("O ID do morador deve ser um valor válido.");
    }

    [Fact]
    public void Validator_DataSaidaFutura_DeveTerErro()
    {
        var command = GetValidCommand();
        command.DataSaida = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.DataSaida).WithErrorMessage("Data de saída não pode ser futura");
    }

    [Fact]
    public void Validator_DataSaidaAnteriorADataEntrada_DeveTerErro()
    {
        var command = GetValidCommand();
        command.DataEntrada = new DateOnly(2023, 10, 20);
        command.DataSaida = new DateOnly(2023, 10, 19);
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.DataSaida).WithErrorMessage("Data de saída deve ser maior que a data de entrada");
    }

    [Fact]
    public void Validator_DataSaidaIgualADataEntrada_DeveTerErro()
    {
        var command = GetValidCommand();
        command.DataEntrada = new DateOnly(2023, 10, 20);
        command.DataSaida = new DateOnly(2023, 10, 20);
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.DataSaida).WithErrorMessage("Data de saída deve ser maior que a data de entrada");
    }

    [Fact]
    public void Validator_DataSaidaValida_NaoDeveTerErros()
    {
        var command = GetValidCommand();
        command.DataEntrada = new DateOnly(2023, 10, 20);
        command.DataSaida = new DateOnly(2023, 10, 21);
        var resultado = _validator.TestValidate(command);
        resultado.ShouldNotHaveValidationErrorFor(c => c.DataSaida);
    }
}
