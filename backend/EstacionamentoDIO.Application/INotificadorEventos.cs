namespace EstacionamentoDIO.Application;

/// <summary>
/// Abstração para notificar clientes em tempo real quando o estado do estacionamento
/// muda. A Application não conhece SignalR — quem implementa isso (a camada Api)
/// decide a tecnologia de transporte.
/// </summary>
public interface INotificadorEventos
{
    Task NotificarEntradaAsync(VeiculoDto veiculo, StatusEstacionamentoDto status, CancellationToken cancellationToken = default);

    Task NotificarSaidaAsync(RegistrarSaidaResultDto resultado, StatusEstacionamentoDto status, CancellationToken cancellationToken = default);
}

/// <summary>Implementação nula, usada quando não há transporte de tempo real configurado (ex.: testes).</summary>
public class NotificadorEventosNulo : INotificadorEventos
{
    public Task NotificarEntradaAsync(VeiculoDto veiculo, StatusEstacionamentoDto status, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task NotificarSaidaAsync(RegistrarSaidaResultDto resultado, StatusEstacionamentoDto status, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
