namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.ValidatorBase;

public interface ICommandBaseImovel
{
    string Bloco { get; set; }
    string Apartamento { get; set; }
    string BoxGaragem { get; set; }
    public long EmpresaId { get; set; }
}