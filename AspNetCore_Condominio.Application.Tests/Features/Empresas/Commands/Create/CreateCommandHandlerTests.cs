using AspNetCore_Condominio.Application.Features.Empresas.Commands.Create;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Empresas.Commands.Create;

public class CreateCommandHandlerTests
{
    private readonly Mock<IEmpresaRepository> _repoMock;
    private readonly CreateCommandHandlerEmpresa _handler;

    public CreateCommandHandlerTests()
    {
        _repoMock = new Mock<IEmpresaRepository>();
        _handler = new CreateCommandHandlerEmpresa(_repoMock.Object);
        _repoMock.Setup(repo => repo.CreateAsync(It.IsAny<Empresa>()))
            .Callback<Empresa>(empresa => empresa.Id = 101)
            .Returns(Task.CompletedTask);
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
            Cnpj = "01.111.222/0001-02",
            TipoDeCondominio = (TipoCondominio)1,
            Nome = "Responsável Atualizado",
            Celular = "(11) 99999-9999",
            Telefone = "(11) 3333-3333",
            Email = "email@gmail.com",
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
        Domain.Common.Result<DTOs.EmpresaDto> resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(idGerado, resultado.Dados.Id);
        Assert.Equal(command.RazaoSocial, resultado.Dados.RazaoSocial);

        _repoMock.Verify(repo => repo.CreateAsync(
            It.Is<Empresa>(
                i => i.RazaoSocial == command.RazaoSocial && i.Cnpj == command.Cnpj
            )),
            Times.Once
        );
    }
}