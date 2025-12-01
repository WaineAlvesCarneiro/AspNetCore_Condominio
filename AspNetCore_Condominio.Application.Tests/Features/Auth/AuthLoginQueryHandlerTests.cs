using AspNetCore_Condominio.Application.Features.Auth;
using AspNetCore_Condominio.Application.Helpers;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Auth;

public class AuthLoginQueryHandlerTests
{
    private readonly Mock<IAuthUserRepository> _authRepoMock;
    private readonly AuthLoginQueryHandler _handler;

    private const string USERNAME = "admin_teste";
    private const string SENHA_CORRETA = "pass123";
    private readonly AuthUser _user;

    //O construtor é executado antes de cada método de teste ([Fact]) e é usado para configurar o ambiente necessário (Arrange).
    //  Aqui, criamos o mock do repositório, configuramos o usuário de teste e instanciamos o handler que será testado.
    //  Isso garante que cada teste comece com um estado limpo e consistente.
    //  O uso do mock permite simular o comportamento do repositório sem depender de uma implementação real,
    //      facilitando o teste isolado do handler.
    public AuthLoginQueryHandlerTests()
    {
        //Cria a instância do mock do repositório.
        _authRepoMock = new Mock<IAuthUserRepository>();

        string hashedPassword = PasswordHasher.HashPassword(SENHA_CORRETA);

        //O objeto _user é criado como o usuário que será retornado pelo mock do repositório em testes onde o login deve ser bem-sucedido.
        //  Ele tem um username e a senha hasheada correta.
        _user = new AuthUser
        {
            Id = Guid.NewGuid(),
            Username = USERNAME,
            PasswordHash = hashedPassword,
            Role = "Admin"
        };
        //Cria a instância do handler que será testado, passando o mock do repositório.
        //  Isso permite que o handler use o mock em vez de um repositório real durante os testes.
        //  Assim, podemos controlar o comportamento do repositório e testar o handler de forma isolada.
        //  O handler é o objeto que contém a lógica de negócio que estamos testando.
        //  Ele processa a consulta de login e interage com o repositório para validar as credenciais.
        _handler = new AuthLoginQueryHandler(_authRepoMock.Object);
    }

    [Fact]
    public async Task Handle_CredenciaisCorretas_DeveRetornarUsuario()
    {
        //Arrange

        // Cria o objeto authLoginQuery com o username e a senha sem hash corretos, simulando o que um usuário enviaria.
        // Configura o mock do repositório para retornar o usuário de teste quando o método GetByUsernameAsync for chamado com o USERNAME correto.
        AuthLoginQuery authLoginQuery = new() { Username = USERNAME, Password = SENHA_CORRETA };

        // Esta é a parte crucial de mocking. Ela instrui o mock do repositório:
        //  "Quando o método GetByUsernameAsync for chamado com o USERNAME correto, retorne o objeto _user que criamos no construtor."
        // Define o comportamento do mock para retornar o usuário quando o nome de usuário correto for fornecido.
        // Isso simula a existência do usuário no repositório.
        // Quando o método GetByUsernameAsync for chamado com USERNAME, ele retornará _user.
        // Isso é essencial para testar o cenário onde o login deve ser bem-sucedido.
        //  Sem essa configuração, o mock retornaria null por padrão, o que não é o comportamento desejado para este teste.
        _authRepoMock.Setup(repo => repo.GetByUsernameAsync(USERNAME)).ReturnsAsync(_user);

        //Act

        // Executa o método do handler que está sendo testado, passando a query de login.
        // Chama o método Handle do handler com a consulta de login e um token de cancelamento nulo.
        // Isso simula o processamento da solicitação de login.
        // O resultado é armazenado na variável resultado.
        AuthUser resultado = await _handler.Handle(authLoginQuery, CancellationToken.None);

        //Assert
        Assert.NotNull(resultado);
        Assert.Equal(_user.Id, resultado.Id);

        // Verification (Verificação de Mock - Opcional, mas boa prática):
        //      Verifica se o método GetByUsernameAsync realmente foi chamado no repositório simulado e se foi chamado exatamente uma vez durante a execução do teste.
        //          Isso garante que o handler usou a dependência conforme o esperado.
        // Verifica se o método GetByUsernameAsync do repositório foi chamado exatamente uma vez com o USERNAME correto.
        // Isso garante que o handler interagiu com o repositório conforme esperado.'
        _authRepoMock.Verify(repo => repo.GetByUsernameAsync(USERNAME), Times.Once);
    }

    [Fact]
    public async Task Handle_UsuarioInexistente_DeveRetornarNull()
    {
        //Arrange
        AuthLoginQuery authLoginQuery = new() { Username = "nao_existe", Password = "any" };
        _authRepoMock.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((AuthUser)null!);

        //Act
        AuthUser resultado = await _handler.Handle(authLoginQuery, CancellationToken.None);

        //Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task Handle_SenhaIncorreta_DeveRetornarNull()
    {
        //Arrange
        AuthLoginQuery authLoginQuery = new() { Username = USERNAME, Password = "senha_errada" };
        _authRepoMock.Setup(repo => repo.GetByUsernameAsync(USERNAME)).ReturnsAsync(_user);

        //Act
        AuthUser resultado = await _handler.Handle(authLoginQuery, CancellationToken.None);

        //Assert
        Assert.Null(resultado);
    }
}