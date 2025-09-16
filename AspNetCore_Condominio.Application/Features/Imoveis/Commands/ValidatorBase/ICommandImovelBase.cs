namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.ValidatorBase;

public interface ICommandImovelBase
{
    string Bloco { get; set; }
    string Apartamento { get; set; }
    string BoxGaragem { get; set; }
}