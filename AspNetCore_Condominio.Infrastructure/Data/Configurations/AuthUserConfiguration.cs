using AspNetCore_Condominio.Domain.Entities.Auth;
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
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier");
        builder.Property(u => u.UserName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Role).HasMaxLength(50).IsRequired();
    }
}
