using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore_Condominio.Domain.Entities;

public class Imovel
{
    public int Id { get; set; }
    public required string Bloco { get; set; }
    public required string Apartamento { get; set; }
    public required string BoxGaragem { get; set; }

    public ICollection<Morador> Moradores { get; set; } = new List<Morador>();
}