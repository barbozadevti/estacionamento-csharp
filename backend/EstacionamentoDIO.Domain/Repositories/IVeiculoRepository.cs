using EstacionamentoDIO.Domain.Entities;

namespace EstacionamentoDIO.Domain.Repositories;

public interface IVeiculoRepository
{
    Task<Veiculo?> ObterEstacionadoPorPlacaAsync(string placa, CancellationToken cancellationToken = default);

    Task<List<Veiculo>> ListarEstacionadosAsync(CancellationToken cancellationToken = default);

    Task<List<Veiculo>> ListarHistoricoAsync(CancellationToken cancellationToken = default);

    Task<int> ContarEstacionadosAsync(CancellationToken cancellationToken = default);

    Task AdicionarAsync(Veiculo veiculo, CancellationToken cancellationToken = default);

    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
