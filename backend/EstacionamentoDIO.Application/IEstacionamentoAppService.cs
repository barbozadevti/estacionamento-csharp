using EstacionamentoDIO.Domain.Entities;

namespace EstacionamentoDIO.Application;

public interface IEstacionamentoAppService
{
    Task<VeiculoDto> RegistrarEntradaAsync(string placa, CancellationToken cancellationToken = default);

    Task<RegistrarSaidaResultDto> RegistrarSaidaAsync(string placa, FormaPagamento formaPagamento, CancellationToken cancellationToken = default);

    Task<List<VeiculoDto>> ListarEstacionadosAsync(CancellationToken cancellationToken = default);

    Task<List<VeiculoDto>> ListarHistoricoAsync(CancellationToken cancellationToken = default);

    Task<StatusEstacionamentoDto> ObterStatusAsync(CancellationToken cancellationToken = default);
}
