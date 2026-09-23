using EstacionamentoDIO.Domain.Entities;
using EstacionamentoDIO.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EstacionamentoDIO.Infrastructure;

public class VeiculoRepository : IVeiculoRepository
{
    private readonly EstacionamentoDbContext contexto;

    public VeiculoRepository(EstacionamentoDbContext contexto)
    {
        this.contexto = contexto;
    }

    public Task<Veiculo?> ObterEstacionadoPorPlacaAsync(string placa, CancellationToken cancellationToken = default) =>
        contexto.Veiculos
            .Where(v => v.Placa == placa && v.HoraSaida == null)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<List<Veiculo>> ListarEstacionadosAsync(CancellationToken cancellationToken = default) =>
        contexto.Veiculos
            .Where(v => v.HoraSaida == null)
            .OrderBy(v => v.HoraEntrada)
            .ToListAsync(cancellationToken);

    public Task<List<Veiculo>> ListarHistoricoAsync(CancellationToken cancellationToken = default) =>
        contexto.Veiculos
            .Where(v => v.HoraSaida != null)
            .OrderByDescending(v => v.HoraSaida)
            .ToListAsync(cancellationToken);

    public Task<List<Veiculo>> ListarSaidasNoPeriodoAsync(DateTime inicioUtc, DateTime fimUtc, CancellationToken cancellationToken = default) =>
        contexto.Veiculos
            .Where(v => v.HoraSaida != null && v.HoraSaida >= inicioUtc && v.HoraSaida < fimUtc)
            .OrderByDescending(v => v.HoraSaida)
            .ToListAsync(cancellationToken);

    public Task<int> ContarEstacionadosAsync(CancellationToken cancellationToken = default) =>
        contexto.Veiculos.CountAsync(v => v.HoraSaida == null, cancellationToken);

    public async Task AdicionarAsync(Veiculo veiculo, CancellationToken cancellationToken = default) =>
        await contexto.Veiculos.AddAsync(veiculo, cancellationToken);

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default) =>
        contexto.SaveChangesAsync(cancellationToken);
}
