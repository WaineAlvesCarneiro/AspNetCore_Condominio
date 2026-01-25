using AspNetCore_Condominio.Application.Features.Events;
using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Events;
using AspNetCore_Condominio.Domain.Interfaces;
using Moq;

namespace AspNetCore_Condominio.Application.Tests.Features.Moradores.Events;

public class CriadoEventHandlerTests
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly CriadoEventHandler<Morador> _handler;

    private readonly Morador _moradorFicticio = new Morador
    {
        Id = 1,
        Nome = "Pedro Eventos",
        Celular = "11991234567",
        Email = "pedro.eventos@teste.com",
        ImovelId = 1
    };

    public CriadoEventHandlerTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _handler = new CriadoEventHandler<Morador>(_emailServiceMock.Object);
    }

    [Fact]
    public async Task Handle_QuandoIsCreateETrue_DeveEnviarEmailDeBoasVindas()
    {
        var notification = new CriadoEventEmail<Morador>(_moradorFicticio, IsCreate: true);
        const string assuntoEsperado = "Bem-vindo ao Condomínio! Asp Net Core";
        await _handler.Handle(notification, CancellationToken.None);
        _emailServiceMock.Verify(service => service.SendAsync(
            _moradorFicticio.Email,
            assuntoEsperado,
            It.Is<string>(corpo => corpo.Contains($"Olá, **{_moradorFicticio.Nome}**! Seu cadastro foi criado com sucesso.")),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoIsCreateEFalse_DeveEnviarEmailDeAtualizacao()
    {
        var notification = new CriadoEventEmail<Morador>(_moradorFicticio, IsCreate: false);
        const string assuntoEsperado = "Atualização de Cadastro no Condomínio! Asp Net Core";
        await _handler.Handle(notification, CancellationToken.None);
        _emailServiceMock.Verify(service => service.SendAsync(
            _moradorFicticio.Email,
            assuntoEsperado,
            It.Is<string>(corpo => corpo.Contains($"Olá, **{_moradorFicticio.Nome}**! Seu cadastro foi atualizado com sucesso.")),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public void Handle_NaoDeveChamarSendAsyncSeLogicaFosseIgnorada()
    {
        var handlerLimpo = new CriadoEventHandler<Morador>(_emailServiceMock.Object);
        _emailServiceMock.Verify(service => service.SendAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()
        ), Times.Never);
    }
}