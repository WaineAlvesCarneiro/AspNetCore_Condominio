using AspNetCore_Condominio.Application.Features.Empresas.Commands.Update;
using AspNetCore_Condominio.Domain.Enums;
using FluentValidation.TestHelper;

namespace AspNetCore_Condominio.Application.Tests.Features.Empresas.Commands.Update;

public class UpdateCommandValidatorTests
{
    private readonly UpdateCommandValidatorEmpresa _validator = new();

    private UpdateCommandEmpresa GetValidCommand() => new()
    {
        Id = 1,
        RazaoSocial = "Razão Social",
        Fantasia = "Fantasia",
        Cnpj = "01111222000102",
        TipoDeCondominio = (TipoCondominio)1,
        Nome = "Responsável",
        Celular = "11999999999",
        Telefone = "1133333333",
        Email = "email@gmail.com",
        Senha = "SenhaForte123!",
        Host = "smtp.exemplo.com",
        Porta = 587,
        Cep = "74843140",
        Uf = "SP",
        Cidade = "São Paulo",
        Endereco = "Rua Exemplo, 123",
        Bairro = "Pq Amazônia",
        Complemento = "Complemento",
        DataInclusao = DateTime.Now
    };

    [Fact]
    public void Validator_ComandoValido_DevePassarSemErros()
    {
        // Arrange
        UpdateCommandEmpresa command = GetValidCommand();
        // Act
        TestValidationResult<UpdateCommandEmpresa> resultado = _validator.TestValidate(command);
        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_RazaoSocialMuitoCurto_DeveFalharComMensagemCorreta()
    {
        string mensagemEsperada = "Razão Social é obrigatória";
        UpdateCommandEmpresa command = GetValidCommand();
        command.RazaoSocial = "";
        TestValidationResult<UpdateCommandEmpresa> resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.RazaoSocial).WithErrorMessage(mensagemEsperada);
    }
}
