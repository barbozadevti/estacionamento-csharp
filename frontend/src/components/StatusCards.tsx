import type { StatusEstacionamento } from '../types/estacionamento'
import { formatarMoeda } from '../utils/format'

interface StatusCardsProps {
  status: StatusEstacionamento | null
  carregando: boolean
}

export function StatusCards({ status, carregando }: StatusCardsProps) {
  const lotado = status !== null && status.vagasDisponiveis <= 0

  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
      <div
        className={`rounded-2xl border p-5 shadow-sm transition-colors ${
          lotado
            ? 'border-amber-300 bg-amber-50'
            : 'border-emerald-200 bg-emerald-50'
        }`}
      >
        <p
          className={`text-xs font-semibold uppercase tracking-wide ${
            lotado ? 'text-amber-700' : 'text-emerald-700'
          }`}
        >
          Vagas disponíveis
        </p>
        <div className="mt-2 flex items-baseline gap-2">
          <span
            className={`text-3xl font-bold tabular-nums ${
              lotado ? 'text-amber-900' : 'text-emerald-900'
            }`}
          >
            {carregando || !status ? '—' : status.vagasDisponiveis}
          </span>
          <span className="text-sm text-slate-500">
            / {carregando || !status ? '—' : status.vagasTotais}
          </span>
        </div>
        {lotado && (
          <p className="mt-1 text-xs font-medium text-amber-700">Estacionamento lotado</p>
        )}
      </div>

      <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
        <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">
          Preço inicial
        </p>
        <div className="mt-2 text-3xl font-bold tabular-nums text-slate-800">
          {carregando || !status ? '—' : formatarMoeda(status.precoInicial)}
        </div>
        <p className="mt-1 text-xs text-slate-400">cobrado na entrada</p>
      </div>

      <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
        <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">
          Preço por hora
        </p>
        <div className="mt-2 text-3xl font-bold tabular-nums text-slate-800">
          {carregando || !status ? '—' : formatarMoeda(status.precoPorHora)}
        </div>
        <p className="mt-1 text-xs text-slate-400">a cada hora adicional</p>
      </div>
    </div>
  )
}
