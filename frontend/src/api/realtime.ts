import * as signalR from '@microsoft/signalr'
import type { RegistrarSaidaResposta, StatusEstacionamento, Veiculo } from '../types/estacionamento'
import { SERVER_URL } from './client'

export type StatusConexao = 'conectando' | 'conectado' | 'reconectando' | 'desconectado'

interface EventosEstacionamento {
  onVeiculoEntrou: (veiculo: Veiculo, status: StatusEstacionamento) => void
  onVeiculoSaiu: (resultado: RegistrarSaidaResposta, status: StatusEstacionamento) => void
  onStatusConexaoMudou: (status: StatusConexao) => void
}

/**
 * Abre a conexão em tempo real com o hub do backend. Retorna uma função
 * de limpeza que encerra a conexão (usar no cleanup de um useEffect).
 */
export function conectarTempoReal(eventos: EventosEstacionamento): () => void {
  const conexao = new signalR.HubConnectionBuilder()
    .withUrl(`${SERVER_URL}/hubs/estacionamento`, { withCredentials: false })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build()

  conexao.on('VeiculoEntrou', eventos.onVeiculoEntrou)
  conexao.on('VeiculoSaiu', eventos.onVeiculoSaiu)

  conexao.onreconnecting(() => eventos.onStatusConexaoMudou('reconectando'))
  conexao.onreconnected(() => eventos.onStatusConexaoMudou('conectado'))
  conexao.onclose(() => eventos.onStatusConexaoMudou('desconectado'))

  eventos.onStatusConexaoMudou('conectando')
  conexao
    .start()
    .then(() => eventos.onStatusConexaoMudou('conectado'))
    .catch(() => eventos.onStatusConexaoMudou('desconectado'))

  return () => {
    conexao.stop()
  }
}
