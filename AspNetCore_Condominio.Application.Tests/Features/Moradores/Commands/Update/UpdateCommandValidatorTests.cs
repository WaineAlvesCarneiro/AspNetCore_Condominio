using AspNetCore_Condominio.Application.Features.Moradores.Commands.Update;
using FluentValidation.TestHelper;
using DateTime = System.DateTime;

namespace AspNetCore_Condominio.Application.Tests.Features.Moradores.Commands.Update;

public class UpdateCommandValidatorTests
{
    private readonly UpdateCommandValidatorMorador _validator = new();
    private readonly DateTime _dataEntradaValida = new DateTime(2023, 1, 10);

    private UpdateCommandMorador GetValidCommand() => new()
    {
        Id = 1,
        Nome = "Novo Nome",
        Celular = "11998765432",
        Email = "novo.email@cond.com",
        IsProprietario = true,
        DataEntrada = DateOnly.FromDateTime(_dataEntradaValida),
        DataInclusao = new DateTime(2023, 1, 10),
        DataSaida = null,
        DataAlteracao = DateTime.Now,
        ImovelId = 1,
        EmpresaId = 1
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
        command.DataSaida = DateOnly.FromDateTime(DateTime.Now).AddDays(1);
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.DataSaida).WithErrorMessage("Data de saída não pode ser futura");
    }

    [Fact]
    public void Validator_DataSaidaAnteriorADataEntrada_DeveTerErro()
    {
        var command = GetValidCommand();
        command.DataEntrada = DateOnly.FromDateTime(new DateTime(2023, 10, 20));
        command.DataSaida = DateOnly.FromDateTime(new DateTime(2023, 10, 19));
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.DataSaida).WithErrorMessage("Data de saída deve ser maior ou igual à data de entrada");
    }

    [Fact]
    public void Validator_DataSaidaValida_NaoDeveTerErros()
    {
        var command = GetValidCommand();
        command.DataEntrada = DateOnly.FromDateTime(new DateTime(2023, 10, 20)  );
        command.DataSaida = DateOnly.FromDateTime(new DateTime(2023, 10, 21)    );
        var resultado = _validator.TestValidate(command);
        resultado.ShouldNotHaveValidationErrorFor(c => c.DataSaida);
    }
}
