using AspNetCore_Condominio.Application.Features.Moradores.Commands.Create;
using FluentValidation.TestHelper;
using DateTime = System.DateTime;

namespace AspNetCore_Condominio.Application.Tests.Features.Moradores.Commands.Create;

public class CreateCommandValidatorTests
{
    private readonly CreateCommandValidatorMorador _validator = new();

    private CreateCommandMorador GetValidCommand() => new()
    {
        Id = 0,
        Nome = "João da Silva",
        Celular = "11998765432",
        Email = "joao.silva@cond.com",
        IsProprietario = true,
        DataEntrada = DateOnly.FromDateTime(DateTime.Now),
        DataInclusao = DateTime.Now,
        DataSaida = null,
        DataAlteracao = null,
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

    [Theory]
    [InlineData("")]
    [InlineData("Jo")]
    [InlineData("A")]
    public void Validator_NomeInvalido_DeveTerErro(string nome)
    {
        var command = GetValidCommand();
        command.Nome = nome.Length > 50 ? new string('A', 51) : nome;
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.Nome);
    }

    [Theory]
    [InlineData("emailinvalido")]
    public void Validator_EmailInvalido_DeveTerErro(string email)
    {
        var command = GetValidCommand();
        command.Email = email;
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.Email).WithErrorMessage("Formato de e-mail inválido");
    }

    [Theory]
    [InlineData("123")]
    public void Validator_CelularInvalido_DeveTerErro(string celular)
    {
        var command = GetValidCommand();
        command.Celular = celular;
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.Celular).WithErrorMessage("O campo Celular precisa ter entre 11 e 16 caracteres");
    }

    [Fact]
    public void Validator_DataEntradaFutura_DeveTerErro()
    {
        var command = GetValidCommand();
        command.DataEntrada = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.DataEntrada).WithErrorMessage("Data de entrada não pode ser futura");
    }

    [Fact]
    public void Validator_ImovelIdZero_DeveTerErro()
    {
        var command = GetValidCommand();
        command.ImovelId = 0;
        var resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.ImovelId).WithErrorMessage("Imóvel é obrigatório");
    }
}