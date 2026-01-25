using AspNetCore_Condominio.Domain.Entities.EmailRemetente;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCore_Condominio.Infrastructure.Data.Configurations;

public class EmailRemetenteConfiguration : IEntityTypeConfiguration<EmailRemetente>
{
    public void Configure(EntityTypeBuilder<EmailRemetente> builder)
    {
        builder.ToTable("EmailRemetente", "dbo");
        builder.HasKey(i => i.Id);
    }
}