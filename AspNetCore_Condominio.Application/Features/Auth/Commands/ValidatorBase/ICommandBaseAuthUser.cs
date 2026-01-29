using AspNetCore_Condominio.Domain.Enums;

namespace AspNetCore_Condominio.Application.Features.Auth.Commands.ValidatorBase;

public interface ICommandBaseAuthUser
{
    long? EmpresaId { get; set; }
    string UserName { get; set; }
    string PasswordHash { get; set; }
    TipoRole Role { get; set; }
    DateTime DataInclusao { get; set; }
    DateTime? DataAlteracao { get; set; }
}