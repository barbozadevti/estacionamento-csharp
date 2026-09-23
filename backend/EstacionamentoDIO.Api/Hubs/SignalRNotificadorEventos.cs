using EstacionamentoDIO.Application;
using Microsoft.AspNetCore.SignalR;

namespace EstacionamentoDIO.Api.Hubs;

public class SignalRNotificadorEventos : INotificadorEventos
{
    private readonly IHubContext<EstacionamentoHub> hub;

    public SignalRNotificadorEventos(IHubContext<EstacionamentoHub> hub)
    {
        this.hub = hub;
    }

    public Task NotificarEntradaAsync(VeiculoDto veiculo, StatusEstacionamentoDto status, CancellationToken cancellationToken = default) =>
        hub.Clients.All.SendAsync("VeiculoEntrou", veiculo, status, cancellationToken);

    public Task NotificarSaidaAsync(RegistrarSaidaResultDto resultado, StatusEstacionamentoDto status, CancellationToken cancellationToken = default) =>
        hub.Clients.All.SendAsync("VeiculoSaiu", resultado, status, cancellationToken);
}
