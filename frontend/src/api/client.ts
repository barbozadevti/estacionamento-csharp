import type {
  ApiErrorBody,
  Estabelecimento,
  FormaPagamento,
  RegistrarSaidaResposta,
  StatusEstacionamento,
  Veiculo,
} from '../types/estacionamento'

const API_URL: string =
  (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:5080/api'

/**
 * Erro de aplicação: representa uma falha de comunicação com a API,
 * já com uma mensagem amigável pronta para exibição na UI.
 */
export class ApiError extends Error {
  status?: number

  constructor(message: string, status?: number) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

async function parseErrorMessage(response: Response): Promise<string> {
  try {
    const body = (await response.json()) as ApiErrorBody
    if (body && typeof body.message === 'string' && body.message.trim() !== '') {
      return body.message
    }
  } catch {
    // corpo não é JSON válido, cai no fallback abaixo
  }
  return `Erro inesperado (HTTP ${response.status}).`
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  let response: Response
  try {
    response = await fetch(`${API_URL}${path}`, {
      ...init,
      headers: {
        'Content-Type': 'application/json',
        ...init?.headers,
      },
    })
  } catch {
    throw new ApiError(
      'Não foi possível conectar ao servidor. Verifique se o backend está em execução.',
    )
  }

  if (!response.ok) {
    const message = await parseErrorMessage(response)
    throw new ApiError(message, response.status)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}

export function buscarStatus(): Promise<StatusEstacionamento> {
  return request<StatusEstacionamento>('/estacionamento/status')
}

export function buscarEstabelecimento(): Promise<Estabelecimento> {
  return request<Estabelecimento>('/estabelecimento')
}

export function listarVeiculosEstacionados(): Promise<Veiculo[]> {
  return request<Veiculo[]>('/veiculos?status=estacionados')
}

export function listarHistoricoVeiculos(): Promise<Veiculo[]> {
  return request<Veiculo[]>('/veiculos?status=historico')
}

export function registrarEntrada(placa: string): Promise<Veiculo> {
  return request<Veiculo>('/veiculos/entrada', {
    method: 'POST',
    body: JSON.stringify({ placa }),
  })
}

export function registrarSaida(
  placa: string,
  formaPagamento: FormaPagamento,
): Promise<RegistrarSaidaResposta> {
  return request<RegistrarSaidaResposta>(`/veiculos/${encodeURIComponent(placa)}/saida`, {
    method: 'POST',
    body: JSON.stringify({ formaPagamento }),
  })
}
