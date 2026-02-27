using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCore_Condominio.Infrastructure.Data.Configurations;

public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("Empresa", "dbo");
        builder.HasKey(i => i.Id);

        builder.Property(u => u.Ativo)
            .HasDefaultValue(TipoEmpresaAtivo.Ativo)
            .IsRequired();

        builder.Property(e => e.RazaoSocial)
            .HasMaxLength(100)
            .IsRequired();
    }
}