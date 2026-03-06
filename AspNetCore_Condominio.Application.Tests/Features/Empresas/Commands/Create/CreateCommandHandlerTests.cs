using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
using AspNetCore_Condominio.Application.Features.Empresas.Commands.Create;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Empresas.Commands.Create;

public class CreateCommandHandlerTests
{
    private readonly Mock<IEmpresaRepository> _repoMock;
    private readonly Mock<IMensageriaService> _mensageriaMock;
    private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
    private readonly Mock<ILogger<CreateCommandHandlerEmpresa>> _loggerMock;
    private readonly CreateCommandHandlerEmpresa _handler;

    public CreateCommandHandlerTests()
    {
        _repoMock = new Mock<IEmpresaRepository>();
        _mensageriaMock = new Mock<IMensageriaService>();
        _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
        _loggerMock = new Mock<ILogger<CreateCommandHandlerEmpresa>>();

        _handler = new CreateCommandHandlerEmpresa(_repoMock.Object, _mensageriaMock.Object, _emailTemplateServiceMock.Object, _loggerMock.Object);

        _repoMock.Setup(repo => repo.CreateAsync(It.IsAny<Empresa>(), It.IsAny<CancellationToken>()))
            .Callback<Empresa, CancellationToken>((empresa, token)=> empresa.Id = 101)
            .Returns(Task.CompletedTask);

        _repoMock.Setup(repo => repo.ExisteCnpjAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }

    [Fact]
    public async Task Handle_ComandoValido_DeveChamarCreateAsyncERetornarSucessoDto()
    {
        // Arrange
        long idGerado = 101;
        CreateCommandEmpresa command = new()
        {
            RazaoSocial = "Razão Social Atualizada",
            Fantasia = "Fantasia Atualizada",
            Cnpj = "44764428000186",
            TipoDeCondominio = (TipoCondominio)1,
            Nome = "Responsável Atualizado",
            Celular = "(11) 99999-9999",
            Telefone = "(11) 3333-3333",
            Email = "enviaemailwebapi@gmail.com",
            Senha = "SenhaForte123!",
            Host = "smtp.exemplo.com",
            Porta = 587,
            Cep = "01234-567",
            Uf = "SP",
            Cidade = "São Paulo",
            Endereco = "Rua Exemplo, 123",
            Bairro = "Pq Amazônia",
            Complemento = "Complemento",
            DataInclusao = DateTime.Now
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(idGerado, resultado.Dados.Id);
        Assert.Equal(command.RazaoSocial, resultado.Dados.RazaoSocial);

        // Verificação do Repositório (Corrigido com o CancellationToken)
        _repoMock.Verify(repo => repo.CreateAsync(
            It.Is<Empresa>(i => i.RazaoSocial == command.RazaoSocial && i.Cnpj == command.Cnpj),
            It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}