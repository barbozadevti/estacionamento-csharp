export interface StatusEstacionamento {
  vagasTotais: number
  vagasDisponiveis: number
  precoInicial: number
  precoPorHora: number
}

export interface Veiculo {
  id: number
  placa: string
  horaEntrada: string
  horaSaida: string | null
  valorCobrado: number | null
}

export interface RegistrarSaidaResposta {
  veiculo: Veiculo
  valorCobrado: number
  horas: number
}

export interface ApiErrorBody {
  message: string
}
