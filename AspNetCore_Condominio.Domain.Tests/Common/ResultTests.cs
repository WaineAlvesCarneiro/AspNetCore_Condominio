using AspNetCore_Condominio.Domain.Common;

namespace AspNetCore_Condominio.Domain.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Success_SemMensagem_DeveRetornarSucessoVerdadeiro()
    {
        var resultado = Result.Success();

        Assert.True(resultado.Sucesso);
        Assert.Null(resultado.Mensagem);
    }

    [Fact]
    public void Success_ComMensagem_DeveRetornarSucessoEMensagem()
    {
        var mensagemEsperada = "Operação concluída com êxito.";
        var resultado = Result.Success(mensagemEsperada);

        Assert.True(resultado.Sucesso);
        Assert.Equal(mensagemEsperada, resultado.Mensagem);
    }

    [Fact]
    public void Failure_DeveRetornarSucessoFalsoEMensagem()
    {
        var mensagemErro = "Ocorreu um erro de validação.";
        var resultado = Result.Failure(mensagemErro);

        Assert.False(resultado.Sucesso);
        Assert.Equal(mensagemErro, resultado.Mensagem);
    }

    [Fact]
    public void ResultGenericoSuccess_DeveRetornarSucessoEDados()
    {
        var dadoEsperado = new { Propriedade = "Teste" };
        var resultado = Result<object>.Success(dadoEsperado);

        Assert.True(resultado.Sucesso);
        Assert.Equal(dadoEsperado, resultado.Dados);
        Assert.Null(resultado.Mensagem);
    }

    [Fact]
    public void ResultGenericoFailure_DeveRetornarSucessoFalsoEMensagem()
    {
        var mensagemErro = "Dados não encontrados.";
        var resultado = Result<int>.Failure(mensagemErro);

        Assert.False(resultado.Sucesso);
        Assert.Equal(mensagemErro, resultado.Mensagem);
        
        Assert.Equal(default, resultado.Dados);
    }
}