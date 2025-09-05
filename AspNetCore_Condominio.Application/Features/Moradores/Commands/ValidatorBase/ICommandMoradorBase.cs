namespace AspNetCore_Condominio.Application.Features.Moradores.Commands.ValidatorBase;

public interface ICommandMoradorBase
{
    string Nome { get; set; }
    string Celular { get; set; }
    string Email { get; set; }
    bool IsProprietario { get; set; }
    DateOnly DataEntrada { get; set; }
    DateOnly DataInclusao { get; set; }
    DateOnly? DataSaida { get; set; }
    DateOnly? DataAlteracao { get; set; }
    int ImovelId { get; set; }
}
