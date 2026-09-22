export type FormaPagamento = 'Dinheiro' | 'Pix' | 'CartaoCredito' | 'CartaoDebito' | 'CarteiraDigital'

export const FORMAS_PAGAMENTO: { valor: FormaPagamento; label: string }[] = [
  { valor: 'Pix', label: 'Pix' },
  { valor: 'CartaoCredito', label: 'Cartão de crédito' },
  { valor: 'CartaoDebito', label: 'Cartão de débito' },
  { valor: 'CarteiraDigital', label: 'Carteira digital' },
  { valor: 'Dinheiro', label: 'Dinheiro' },
]

export function rotuloFormaPagamento(forma: FormaPagamento | null): string {
  return FORMAS_PAGAMENTO.find((f) => f.valor === forma)?.label ?? '—'
}

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
  formaPagamento: FormaPagamento | null
}

export interface RegistrarSaidaResposta {
  veiculo: Veiculo
  valorCobrado: number
  horas: number
  formaPagamento: FormaPagamento
}

export interface Estabelecimento {
  nome: string
  cnpj: string
}

export interface ApiErrorBody {
  message: string
}
