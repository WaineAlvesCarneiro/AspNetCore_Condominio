using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Entities.Auth;
using AspNetCore_Condominio.Domain.Entities.EmailRemetente;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore_Condominio.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<AuthUser> AuthUsers { get; set; }
    public DbSet<EmailRemetente> EmailRemetentes { get; set; }
    public DbSet<Imovel> Imovels { get; set; }
    public DbSet<Morador> Moradors { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTime>()
            .HaveColumnType("timestamp without time zone");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AuthUser>().ToTable("authusers", "public");

        modelBuilder.Entity<AuthUser>().HasKey(u => u.Id);

        modelBuilder.Entity<AuthUser>().Property(u => u.Id).HasColumnName("id").HasColumnType("uuid");
        modelBuilder.Entity<AuthUser>().Property(u => u.Username).HasColumnName("username").IsRequired().HasMaxLength(100);
        modelBuilder.Entity<AuthUser>().Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired();
        modelBuilder.Entity<AuthUser>().Property(u => u.Role).HasColumnName("role").IsRequired().HasMaxLength(50);

        modelBuilder.Entity<EmailRemetente>().ToTable("emailremetente", "public");

        modelBuilder.Entity<EmailRemetente>().HasKey(er => er.Id);

        modelBuilder.Entity<EmailRemetente>().Property(er => er.Id).HasColumnName("id").HasColumnType("bigint");
        modelBuilder.Entity<EmailRemetente>().Property(er => er.Username).HasColumnName("username").IsRequired();
        modelBuilder.Entity<EmailRemetente>().Property(er => er.Senha).HasColumnName("senha").IsRequired();
        modelBuilder.Entity<EmailRemetente>().Property(er => er.Host).HasColumnName("host").IsRequired();
        modelBuilder.Entity<EmailRemetente>().Property(er => er.Port).HasColumnName("port").IsRequired();

        modelBuilder.Entity<Imovel>().ToTable("imovel", "public");

        modelBuilder.Entity<Imovel>().HasKey(i => i.Id);

        modelBuilder.Entity<Imovel>().Property(i => i.Id).HasColumnName("id").HasColumnType("bigint");
        modelBuilder.Entity<Imovel>().Property(i => i.Bloco).HasColumnName("bloco").IsRequired();
        modelBuilder.Entity<Imovel>().Property(i => i.Apartamento).HasColumnName("apartamento").IsRequired();
        modelBuilder.Entity<Imovel>().Property(i => i.BoxGaragem).HasColumnName("box_garagem").IsRequired();

        modelBuilder.Entity<Morador>().ToTable("morador", "public");

        modelBuilder.Entity<Morador>().HasKey(m => m.Id);

        modelBuilder.Entity<Morador>().Property(m => m.Id).HasColumnName("id").HasColumnType("bigint");
        modelBuilder.Entity<Morador>().Property(m => m.Nome).HasColumnName("nome").IsRequired();
        modelBuilder.Entity<Morador>().Property(m => m.Celular).HasColumnName("celular").IsRequired();
        modelBuilder.Entity<Morador>().Property(m => m.Email).HasColumnName("email").IsRequired();
        modelBuilder.Entity<Morador>().Property(m => m.IsProprietario).HasColumnName("is_proprietario").IsRequired();
        modelBuilder.Entity<Morador>().Property(m => m.DataEntrada).HasColumnName("data_entrada").IsRequired();
        modelBuilder.Entity<Morador>().Property(m => m.DataSaida).HasColumnName("data_saida");
        modelBuilder.Entity<Morador>().Property(m => m.DataInclusao).HasColumnName("data_inclusao").IsRequired();
        modelBuilder.Entity<Morador>().Property(m => m.DataAlteracao).HasColumnName("data_alteracao");
        modelBuilder.Entity<Morador>().Property(m => m.ImovelId).HasColumnName("imovel_id").IsRequired();

        modelBuilder.Entity<Morador>()
            .HasOne(m => m.Imovel)
            .WithMany(i => i.Moradores)
            .HasForeignKey(m => m.ImovelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}
