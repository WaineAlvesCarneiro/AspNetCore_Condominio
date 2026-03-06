using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.Create;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.Update;
using AspNetCore_Condominio.Domain.Entities;

namespace AspNetCore_Condominio.Application.Mappings;

public static class MoradorMappingExtensions
{
    public static Morador ToEntity(this CreateCommandMorador request) => new()
    {
        Nome = request.Nome,
        Celular = request.Celular,
        Email = request.Email,
        IsProprietario = request.IsProprietario,
        DataEntrada = request.DataEntrada,
        DataInclusao = DateTime.Now,
        ImovelId = request.ImovelId,
        EmpresaId = request.EmpresaId
    };

    public static void UpdateFromCommand(this Morador entidade, UpdateCommandMorador request)
    {
        entidade.Nome = request.Nome;
        entidade.Celular = request.Celular;
        entidade.Email = request.Email;
        entidade.IsProprietario = request.IsProprietario;
        entidade.DataEntrada = request.DataEntrada;
        entidade.DataSaida = request.DataSaida;
        entidade.ImovelId = request.ImovelId;
        entidade.DataAlteracao = DateTime.Now;
    }

    public static MoradorDto ToDto(this Morador entidade) => new()
    {
        Id = entidade.Id,
        Nome = entidade.Nome,
        Celular = entidade.Celular,
        Email = entidade.Email,
        IsProprietario = entidade.IsProprietario,
        DataEntrada = entidade.DataEntrada,
        DataSaida = entidade.DataSaida,
        DataInclusao = entidade.DataInclusao,
        DataAlteracao = entidade.DataAlteracao,
        ImovelId = entidade.ImovelId,
        EmpresaId = entidade.EmpresaId,
        ImovelDto = entidade.Imovel?.ToDto(),
        EmpresaDto = entidade.Empresa?.ToDto()
    };
}