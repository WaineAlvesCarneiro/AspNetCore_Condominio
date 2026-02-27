namespace AspNetCore_Condominio.Domain.Entities.Auth;

public record AuthLoginRequest(
    string Username,
    string Password
);