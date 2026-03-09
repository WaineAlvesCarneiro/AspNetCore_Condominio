using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Imoveis.Commands.Create;
using AspNetCore_Condominio.Application.Features.Imoveis.Commands.Update;
using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Application.Mappings;

public static class ImovelMappingExtensions
{
    public static Imovel ToEntity(this CreateCommandImovel request) => new()
    {
        Bloco = request.Bloco,
        Apartamento = request.Apartamento,
        BoxGaragem = request.BoxGaragem,
        EmpresaId = request.EmpresaId
    };

    public static void UpdateFromCommand(this Imovel imovel, UpdateCommandImovel request)
    {
        imovel.Bloco = request.Bloco;
        imovel.Apartamento = request.Apartamento;
        imovel.BoxGaragem = request.BoxGaragem;
    }

    public static ImovelDto ToDto(this Imovel imovel) => new()
    {
        Id = imovel.Id,
        Bloco = imovel.Bloco,
        Apartamento = imovel.Apartamento,
        BoxGaragem = imovel.BoxGaragem,
        EmpresaId = imovel.EmpresaId,
        EmpresaDto = imovel.Empresa?.ToDto()
    };
}