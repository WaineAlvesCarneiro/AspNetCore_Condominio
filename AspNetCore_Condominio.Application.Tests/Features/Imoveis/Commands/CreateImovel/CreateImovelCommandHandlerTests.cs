using AspNetCore_Condominio.Application.Features.Imoveis.Commands.CreateImovel;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Repositories;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Imoveis.Commands.CreateImovel;

public class CreateImovelCommandHandlerTests
{
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly CreateImovelCommandHandler _handler;

    //O objetivo principal deste teste é garantir que:
    //    O handler chame o método de criação no repositório(IImovelRepository.CreateAsync) exatamente uma vez com os dados corretos.
    //    O handler retorne um resultado de sucesso(Sucesso = true) contendo o objeto do imóvel criado, incluindo o ID gerado.
    public CreateImovelCommandHandlerTests()
    {
        //Cria um mock da interface IImovelRepository. Isso permite simular o comportamento do repositório sem depender da implementação real (como um banco de dados).
        // Configuração do Mock do Repositório
        _imovelRepoMock = new Mock<IImovelRepository>();

        //Instancia o handler CreateImovelCommandHandler, passando o objeto mock do repositório.
        // Isso permite testar o handler isoladamente, controlando o comportamento do repositório através do mock.
        // Configuração do Handler com o Mock
        _handler = new CreateImovelCommandHandler(_imovelRepoMock.Object);

        //Define o comportamento simulado para o método CreateAsync quando chamado com qualquer objeto Imovel.
        //Configura o mock para que, quando o método CreateAsync for chamado com qualquer objeto Imovel,
        //  ele atribua um ID ao imóvel (simulando a criação no banco de dados) e retorne uma tarefa concluída.
        _imovelRepoMock.Setup(repo => repo.CreateAsync(It.IsAny<Imovel>()))
            //.Callback<Imovel>(imovel => imovel.Id = 101):
            //  Define uma ação a ser executada quando o método simulado for chamado.
            //  Neste caso, ele simula o banco de dados/repositório atribuindo um ID (101) ao objeto Imovel que foi passado para o método.
            .Callback<Imovel>(imovel => imovel.Id = 101)
            //.Returns(Task.CompletedTask) 
            //  Indica que o método CreateAsync retorna uma tarefa concluída (Task.CompletedTask), simulando uma operação assíncrona bem-sucedida.
            //  Isso é importante para manter a assinatura assíncrona do método.
            //  Em resumo, essa configuração do mock permite que o teste verifique se o handler está
            //  chamando o método CreateAsync corretamente e se está lidando com a criação do imóvel conforme esperado.
            // .Returns(Task.CompletedTask):
            //  Garante que o método simulado retorne um Task completo(simulando uma operação assíncrona bem - sucedida).
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Handle_ComandoValido_DeveChamarCreateAsyncERetornarSucessoDto()
    {
        // Arrange
        int idGerado = 101;
        // Cria um comando válido com os dados necessários para criar um imóvel.
        // Esses dados serão usados para testar o handler.
        CreateImovelCommand command = new()
        {
            Bloco = "Bloco B",
            Apartamento = "202",
            BoxGaragem = "B2"
        };

        // Act
        // Chama o método Handle do handler com o comando criado.
        // Aguarda o resultado da operação.
        Domain.Common.Result<DTOs.ImovelDto> resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(idGerado, resultado.Dados.Id);
        Assert.Equal(command.Bloco, resultado.Dados.Bloco);

        // Verifica se o método CreateAsync foi chamado exatamente uma vez com os dados corretos.
        // Isso garante que o handler está interagindo corretamente com o repositório.
        //Verifica se o método repo.CreateAsync foi chamado exatamente uma vez (Times.Once)
        //  e se o objeto Imovel passado como argumento tinha os valores corretos (i.Bloco == command.Bloco e i.Apartamento == command.Apartamento).
        _imovelRepoMock.Verify(repo => repo.CreateAsync(
            It.Is<Imovel>(
                i => i.Bloco == command.Bloco && i.Apartamento == command.Apartamento
            )),
            Times.Once
        );
    }
}