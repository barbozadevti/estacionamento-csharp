using Microsoft.AspNetCore.SignalR;

namespace EstacionamentoDIO.Api.Hubs;

/// <summary>
/// Hub de tempo real: o servidor só transmite eventos (entrada/saída de veículos),
/// os clientes não precisam invocar nenhum método — só escutar.
/// </summary>
public class EstacionamentoHub : Hub
{
}
