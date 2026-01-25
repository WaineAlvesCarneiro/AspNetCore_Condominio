namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.ValidatorBase;

public interface ICommandBaseMorador
{
    string Nome { get; set; }
    string Celular { get; set; }
    string Email { get; set; }
    bool IsProprietario { get; set; }
    DateOnly DataEntrada { get; set; }
    DateTime DataInclusao { get; set; }
    DateOnly? DataSaida { get; set; }
    DateTime? DataAlteracao { get; set; }
    long ImovelId { get; set; }
    long EmpresaId { get; set; }
}
