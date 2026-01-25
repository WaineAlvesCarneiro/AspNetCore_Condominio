namespace AspNetCore_Condominio.Domain.Common;

public class Result
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }

    public static Result Success(string? mensagem = null) =>
        new() { Sucesso = true, Mensagem = mensagem };

    public static Result Failure(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem };
}

public class Result<T> : Result
{
    public T? Dados { get; set; }

    public static Result<T> Success(T dados, string? mensagem = null) =>
        new() { Sucesso = true, Dados = dados, Mensagem = mensagem };

    public static new Result<T> Failure(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem };
}
