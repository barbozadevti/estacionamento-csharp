const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

const dateTimeFormatter = new Intl.DateTimeFormat('pt-BR', {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
})

export function formatarMoeda(valor: number): string {
  return currencyFormatter.format(valor)
}

export function formatarDataHora(iso: string): string {
  return dateTimeFormatter.format(new Date(iso))
}

/**
 * Calcula o tempo decorrido entre uma data ISO e agora,
 * formatado como "Hh Mmin Ss".
 */
export function formatarTempoDecorrido(horaEntradaIso: string, agora: Date = new Date()): string {
  const inicio = new Date(horaEntradaIso).getTime()
  const diffMs = Math.max(0, agora.getTime() - inicio)
  const totalSegundos = Math.floor(diffMs / 1000)

  const horas = Math.floor(totalSegundos / 3600)
  const minutos = Math.floor((totalSegundos % 3600) / 60)
  const segundos = totalSegundos % 60

  const pad = (n: number) => String(n).padStart(2, '0')

  if (horas > 0) {
    return `${horas}h ${pad(minutos)}min ${pad(segundos)}s`
  }
  return `${minutos}min ${pad(segundos)}s`
}
