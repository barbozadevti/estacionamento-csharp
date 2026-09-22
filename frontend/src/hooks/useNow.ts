import { useEffect, useState } from 'react'

/**
 * Retorna a data/hora atual, atualizada a cada `intervaloMs`.
 * Usado para recalcular o tempo decorrido de cada veículo estacionado
 * no cliente, sem precisar rebuscar dados do servidor.
 */
export function useNow(intervaloMs = 1000): Date {
  const [agora, setAgora] = useState(() => new Date())

  useEffect(() => {
    const id = window.setInterval(() => setAgora(new Date()), intervaloMs)
    return () => window.clearInterval(id)
  }, [intervaloMs])

  return agora
}
