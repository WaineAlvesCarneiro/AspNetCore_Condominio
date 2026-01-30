using AspNetCore_Condominio.Application.Features.Empresas.Commands.Create;
using AspNetCore_Condominio.Domain.Enums;
using FluentValidation.TestHelper;

namespace AspNetCore_Condominio.Application.Tests.Features.Empresas.Commands.Create;

public class CreateCommandValidatorTests
{
    private readonly CreateCommandValidatorEmpresa _validator;

    public CreateCommandValidatorTests()
    {
        _validator = new CreateCommandValidatorEmpresa();
    }

    private CreateCommandEmpresa GetValidCommand() => new()
    {
        RazaoSocial = "Razão Social Atualizada",
        Fantasia = "Fantasia Atualizada",
        Cnpj = "01111222000102",
        TipoDeCondominio = (TipoCondominio)1,
        Nome = "Responsável Atualizado",
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
        DataInclusao = DateTime.UtcNow
    };

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validator_RazaoSocialVazioOuNulo_DeveTerErro(string razaoSocialInvalida)
    {
        // Arrange
        string messagemEsperada = "Razão Social é obrigatória";
        CreateCommandEmpresa command = GetValidCommand();
        command.RazaoSocial = razaoSocialInvalida;

        // Act
        TestValidationResult<CreateCommandEmpresa> resultado = _validator.TestValidate(command);

        // Assert
        resultado.ShouldHaveValidationErrorFor(c => c.RazaoSocial).WithErrorMessage(messagemEsperada);
    }

    [Fact]
    public void Validator_Cnpj_Vazio_DeveTerErro()
    {
        // Arrange
        string messagemEsperada = "Cnpj é obrigatório";
        CreateCommandEmpresa command = GetValidCommand();
        command.Cnpj = "";
        // Act
        TestValidationResult<CreateCommandEmpresa> resultado = _validator.TestValidate(command);
        // Assert
        resultado.ShouldHaveValidationErrorFor(c => c.Cnpj).WithErrorMessage(messagemEsperada);
    }

    [Fact]
    public void Validator_ComandoValido_NaoDeveTerErros()
    {
        // Arrange
        CreateCommandEmpresa command = GetValidCommand();
        // Act
        TestValidationResult<CreateCommandEmpresa> resultado = _validator.TestValidate(command);
        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }
}
