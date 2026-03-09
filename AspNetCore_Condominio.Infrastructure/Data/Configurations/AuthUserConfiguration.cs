using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCore_Condominio.Infrastructure.Data.Configurations;

public class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> builder)
    {
        builder.ToTable("AuthUsers", "dbo");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(u => u.Ativo)
            .HasDefaultValue(TipoUserAtivo.Ativo)
            .IsRequired();

        builder.Property(u => u.EmpresaAtiva)
            .HasDefaultValue(TipoEmpresaAtivo.Ativo)
            .IsRequired();

        builder.Property(u => u.UserName)
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(u => u.UserName).IsUnique();

        builder.Property(u => u.Role)
            .IsRequired();
    }
}