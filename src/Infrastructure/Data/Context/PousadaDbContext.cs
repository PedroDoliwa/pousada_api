using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Entities;

namespace PousadaApi.Infrastructure.Data.Context;

public class PousadaDbContext : DbContext
{
    public PousadaDbContext(DbContextOptions<PousadaDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Pousada> Pousadas { get; set; }
    public DbSet<Quarto> Quartos { get; set; }
    public DbSet<Hospede> Hospedes { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<CalendarioExterno> CalendariosExternos { get; set; }
    public DbSet<UsuarioRecuperacaoSenha> UsuarioRecuperacaoSenhas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.SenhaHash).IsRequired();
            entity.Property(e => e.Perfil).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FotoContentType).HasMaxLength(50);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasMany(e => e.Pousadas).WithOne(p => p.Usuario).HasForeignKey(p => p.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UsuarioRecuperacaoSenha>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TokenHash).IsRequired().HasMaxLength(128);
            entity.HasIndex(e => e.TokenHash);
            entity.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Pousada>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Endereco).IsRequired().HasMaxLength(250);
            entity.Property(e => e.Telefone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.FotoContentType).HasMaxLength(50);
            entity.Property(e => e.UsuarioId).IsRequired();
            entity.HasMany(e => e.Quartos).WithOne(q => q.Pousada).HasForeignKey(q => q.PousadaId);
        });

        modelBuilder.Entity<Quarto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroOuNome).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TokenExportacao).IsRequired().HasMaxLength(64);
            entity.HasIndex(e => e.TokenExportacao).IsUnique();
            entity.HasIndex(e => e.PousadaId);
            entity.HasMany(e => e.Reservas).WithOne(r => r.Quarto).HasForeignKey(r => r.QuartoId);
            entity.HasMany(e => e.CalendariosExternos).WithOne(c => c.Quarto).HasForeignKey(c => c.QuartoId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Hospede>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.PousadaId).IsRequired();
            entity.HasIndex(e => e.PousadaId);
            entity.HasOne(e => e.Pousada).WithMany(p => p.Hospedes).HasForeignKey(e => e.PousadaId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Reservas).WithOne(r => r.Hospede).HasForeignKey(r => r.HospedeId);
        });

        modelBuilder.Entity<CalendarioExterno>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Canal).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UrlImportacao).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.UltimoErro).HasMaxLength(2000);
            entity.HasIndex(e => e.QuartoId);
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Origem).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UidExterno).HasMaxLength(500);
            entity.Property(e => e.TituloExterno).HasMaxLength(500);
            entity.Property(e => e.DataEntrada).IsRequired();
            entity.Property(e => e.DataSaida).IsRequired();
            entity.HasIndex(e => new { e.DataEntrada, e.DataSaida });
            entity.HasIndex(e => new { e.QuartoId, e.UidExterno }).IsUnique().HasFilter("\"UidExterno\" IS NOT NULL");
            entity.HasOne(e => e.CalendarioExterno).WithMany(c => c.Reservas).HasForeignKey(e => e.CalendarioExternoId).OnDelete(DeleteBehavior.SetNull);
        });
    }
}
