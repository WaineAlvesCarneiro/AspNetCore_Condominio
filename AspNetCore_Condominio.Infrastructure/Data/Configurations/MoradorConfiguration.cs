using AspNetCore_Condominio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCore_Condominio.Infrastructure.Data.Configurations;

public class MoradorConfiguration : IEntityTypeConfiguration<Morador>
{
    public void Configure(EntityTypeBuilder<Morador> builder)
    {
        builder.ToTable("Morador", "dbo");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Nome)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("Nome");

        builder.Property(m => m.Celular)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(m => m.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(m => m.IsProprietario)
            .IsRequired();

        builder.Property(m => m.DataEntrada)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(m => m.DataSaida)
            .HasColumnType("date");

        builder.Property(m => m.DataInclusao)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(m => m.DataAlteracao)
            .HasColumnType("datetime2");

        builder.Property(m => m.ImovelId)
            .IsRequired();

        builder.Property(m => m.EmpresaId)
            .HasColumnType("bigint")
            .IsRequired();

        builder.HasOne(m => m.Imovel)
            .WithMany(i => i.Moradores)
            .HasForeignKey(m => m.ImovelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(m => m.Empresa)
                    .WithMany()
                    .HasForeignKey(m => m.EmpresaId)
                    .OnDelete(DeleteBehavior.NoAction);

        builder.HasCheckConstraint(
            "CK_Morador_DataSaida",
            "[DataSaida] IS NULL OR [DataSaida] >= [DataEntrada]");
    }
}