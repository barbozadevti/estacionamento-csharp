import { useEffect, useState } from 'react'
import { buscarRelatorioFaturamento } from '../api/client'
import { ApiError } from '../api/client'
import { rotuloFormaPagamento } from '../types/estacionamento'
import type { RelatorioFaturamento } from '../types/estacionamento'
import { formatarMoeda } from '../utils/format'

type Periodo = 'hoje' | 'ultimos7dias' | 'ultimos30dias'

const PERIODOS: { valor: Periodo; label: string }[] = [
  { valor: 'hoje', label: 'Hoje' },
  { valor: 'ultimos7dias', label: 'Últimos 7 dias' },
  { valor: 'ultimos30dias', label: 'Últimos 30 dias' },
]

function calcularIntervalo(periodo: Periodo): { inicio: Date; fim: Date } {
  const fim = new Date()
  const inicio = new Date()
  inicio.setHours(0, 0, 0, 0)

  if (periodo === 'ultimos7dias') {
    inicio.setDate(inicio.getDate() - 6)
  } else if (periodo === 'ultimos30dias') {
    inicio.setDate(inicio.getDate() - 29)
  }

  return { inicio, fim }
}

export function RelatorioPanel() {
  const [periodo, setPeriodo] = useState<Periodo>('hoje')
  const [relatorio, setRelatorio] = useState<RelatorioFaturamento | null>(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState<string | null>(null)

  useEffect(() => {
    let cancelado = false
    setCarregando(true)
    setErro(null)

    const { inicio, fim } = calcularIntervalo(periodo)

    buscarRelatorioFaturamento(inicio, fim)
      .then((resp) => {
        if (!cancelado) setRelatorio(resp)
      })
      .catch((err) => {
        if (!cancelado) {
          setErro(err instanceof ApiError ? err.message : 'Erro ao carregar o relatório.')
        }
      })
      .finally(() => {
        if (!cancelado) setCarregando(false)
      })

    return () => {
      cancelado = true
    }
  }, [periodo])

  return (
    <div className="flex flex-col gap-4">
      <div className="inline-flex gap-1 self-start rounded-xl bg-slate-200/70 p-1">
        {PERIODOS.map((p) => (
          <button
            key={p.valor}
            type="button"
            onClick={() => setPeriodo(p.valor)}
            className={`rounded-lg px-3 py-1.5 text-xs font-semibold transition ${
              periodo === p.valor
                ? 'bg-white text-blue-700 shadow-sm'
                : 'text-slate-600 hover:text-slate-900'
            }`}
          >
            {p.label}
          </button>
        ))}
      </div>

      {erro && (
        <div className="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700">
          {erro}
        </div>
      )}

      {carregando ? (
        <div className="rounded-2xl border border-slate-200 bg-white p-8 text-center text-sm text-slate-400 shadow-sm">
          Carregando relatório...
        </div>
      ) : (
        relatorio && (
          <>
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
                <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">
                  Total arrecadado
                </p>
                <p className="mt-2 text-3xl font-bold text-emerald-600">
                  {formatarMoeda(relatorio.totalArrecadado)}
                </p>
              </div>
              <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
                <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">
                  Veículos atendidos
                </p>
                <p className="mt-2 text-3xl font-bold text-slate-900">{relatorio.totalVeiculos}</p>
              </div>
            </div>

            <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
              <div className="border-b border-slate-100 px-4 py-3 text-xs font-semibold uppercase tracking-wide text-slate-500">
                Faturamento por forma de pagamento
              </div>
              {relatorio.porFormaPagamento.length === 0 ? (
                <div className="p-8 text-center text-sm text-slate-400">
                  Nenhuma saída registrada no período.
                </div>
              ) : (
                <ul className="divide-y divide-slate-100">
                  {relatorio.porFormaPagamento.map((item) => {
                    const percentual =
                      relatorio.totalArrecadado > 0
                        ? (item.total / relatorio.totalArrecadado) * 100
                        : 0
                    return (
                      <li key={item.formaPagamento} className="px-4 py-3">
                        <div className="flex items-center justify-between text-sm">
                          <span className="font-medium text-slate-700">
                            {rotuloFormaPagamento(item.formaPagamento)}{' '}
                            <span className="text-slate-400">· {item.quantidade}x</span>
                          </span>
                          <span className="font-semibold tabular-nums text-slate-900">
                            {formatarMoeda(item.total)}
                          </span>
                        </div>
                        <div className="mt-1.5 h-1.5 w-full overflow-hidden rounded-full bg-slate-100">
                          <div
                            className="h-full rounded-full bg-blue-500"
                            style={{ width: `${percentual}%` }}
                          />
                        </div>
                      </li>
                    )
                  })}
                </ul>
              )}
            </div>
          </>
        )
      )}
    </div>
  )
}
