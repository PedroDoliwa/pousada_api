using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Entities;

namespace PousadaApi.Infrastructure.Data;

public class PousadaDbContext : DbContext
{
    public PousadaDbContext(DbContextOptions<PousadaDbContext> options) : base(options)
    {
    }

    public DbSet<Pousada> Pousadas { get; set; }
    public DbSet<Quarto> Quartos { get; set; }
    public DbSet<Hospede> Hospedes { get; set; }
    public DbSet<Reserva> Reservas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Pousada
        modelBuilder.Entity<Pousada>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Endereco).IsRequired().HasMaxLength(250);
            entity.Property(e => e.Telefone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasMany(e => e.Quartos).WithOne(q => q.Pousada).HasForeignKey(q => q.PousadaId);
        });

        // Quarto
        modelBuilder.Entity<Quarto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroOuNome).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.PousadaId);
            entity.HasMany(e => e.Reservas).WithOne(r => r.Quarto).HasForeignKey(r => r.QuartoId);
        });

        // Hospede
        modelBuilder.Entity<Hospede>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.HasMany(e => e.Reservas).WithOne(r => r.Hospede).HasForeignKey(r => r.HospedeId);
        });

        // Reserva
        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DataEntrada).IsRequired();
            entity.Property(e => e.DataSaida).IsRequired();
            entity.HasIndex(e => new { e.DataEntrada, e.DataSaida });
        });
    }
}
