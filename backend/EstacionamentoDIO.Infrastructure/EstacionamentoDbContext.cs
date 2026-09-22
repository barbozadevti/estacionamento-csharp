using EstacionamentoDIO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EstacionamentoDIO.Infrastructure;

public class EstacionamentoDbContext : DbContext
{
    public EstacionamentoDbContext(DbContextOptions<EstacionamentoDbContext> options) : base(options)
    {
    }

    public DbSet<Veiculo> Veiculos => Set<Veiculo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // SQLite não guarda o Kind do DateTime; sem isso, ao reler HoraEntrada/HoraSaida
        // o valor volta como Unspecified e é serializado sem o sufixo "Z", quebrando o
        // cálculo de tempo decorrido no frontend (que assume horários em UTC).
        var conversorDataUtc = new ValueConverter<DateTime, DateTime>(
            paraBanco => paraBanco,
            doBanco => DateTime.SpecifyKind(doBanco, DateTimeKind.Utc));

        var conversorDataUtcNullable = new ValueConverter<DateTime?, DateTime?>(
            paraBanco => paraBanco,
            doBanco => doBanco.HasValue ? DateTime.SpecifyKind(doBanco.Value, DateTimeKind.Utc) : doBanco);

        modelBuilder.Entity<Veiculo>(entidade =>
        {
            entidade.ToTable("Veiculos");
            entidade.HasKey(v => v.Id);
            entidade.Property(v => v.Placa).IsRequired().HasMaxLength(10);
            entidade.Property(v => v.HoraEntrada).IsRequired().HasConversion(conversorDataUtc);
            entidade.Property(v => v.HoraSaida).HasConversion(conversorDataUtcNullable);
            entidade.Property(v => v.ValorCobrado).HasColumnType("decimal(10,2)");
            entidade.HasIndex(v => new { v.Placa, v.HoraSaida });
        });
    }
}
