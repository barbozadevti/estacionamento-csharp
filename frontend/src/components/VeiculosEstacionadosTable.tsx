import { useNow } from '../hooks/useNow'
import type { Veiculo } from '../types/estacionamento'
import { formatarDataHora, formatarTempoDecorrido } from '../utils/format'

interface VeiculosEstacionadosTableProps {
  veiculos: Veiculo[]
  onAbrirPagamento: (placa: string) => void
  placasProcessando: Set<string>
  carregando: boolean
}

export function VeiculosEstacionadosTable({
  veiculos,
  onAbrirPagamento,
  placasProcessando,
  carregando,
}: VeiculosEstacionadosTableProps) {
  const agora = useNow(1000)

  if (carregando) {
    return (
      <div className="rounded-2xl border border-slate-200 bg-white p-8 text-center text-sm text-slate-400 shadow-sm">
        Carregando veículos estacionados...
      </div>
    )
  }

  if (veiculos.length === 0) {
    return (
      <div className="rounded-2xl border border-dashed border-slate-300 bg-white p-8 text-center text-sm text-slate-400 shadow-sm">
        Nenhum veículo estacionado no momento.
      </div>
    )
  }

  return (
    <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
      <table className="w-full text-left text-sm">
        <thead className="bg-slate-50 text-xs uppercase tracking-wide text-slate-500">
          <tr>
            <th scope="col" className="px-4 py-3 font-semibold">
              Placa
            </th>
            <th scope="col" className="px-4 py-3 font-semibold">
              Entrada
            </th>
            <th scope="col" className="px-4 py-3 font-semibold">
              Tempo estacionado
            </th>
            <th scope="col" className="px-4 py-3 font-semibold text-right">
              Ação
            </th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100">
          {veiculos.map((veiculo) => {
            const processando = placasProcessando.has(veiculo.placa)
            return (
              <tr key={veiculo.id} className="transition-colors hover:bg-slate-50">
                <td className="px-4 py-3 font-mono text-base font-semibold tracking-wider text-slate-800">
                  {veiculo.placa}
                </td>
                <td className="px-4 py-3 text-slate-600">
                  {formatarDataHora(veiculo.horaEntrada)}
                </td>
                <td className="px-4 py-3 tabular-nums text-slate-600">
                  {formatarTempoDecorrido(veiculo.horaEntrada, agora)}
                </td>
                <td className="px-4 py-3 text-right">
                  <button
                    type="button"
                    disabled={processando}
                    onClick={() => onAbrirPagamento(veiculo.placa)}
                    className="inline-flex items-center gap-2 rounded-lg border border-red-200 bg-red-50 px-3 py-1.5 text-xs font-semibold text-red-700 transition hover:bg-red-100 disabled:cursor-not-allowed disabled:opacity-50"
                  >
                    {processando && (
                      <span className="h-3 w-3 animate-spin rounded-full border-2 border-red-400/40 border-t-red-600" />
                    )}
                    {processando ? 'Registrando...' : 'Registrar saída'}
                  </button>
                </td>
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}
