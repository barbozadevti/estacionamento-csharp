import type { Veiculo } from '../types/estacionamento'
import { formatarDataHora, formatarMoeda } from '../utils/format'

interface HistoricoTableProps {
  veiculos: Veiculo[]
  carregando: boolean
}

export function HistoricoTable({ veiculos, carregando }: HistoricoTableProps) {
  if (carregando) {
    return (
      <div className="rounded-2xl border border-slate-200 bg-white p-8 text-center text-sm text-slate-400 shadow-sm">
        Carregando histórico...
      </div>
    )
  }

  if (veiculos.length === 0) {
    return (
      <div className="rounded-2xl border border-dashed border-slate-300 bg-white p-8 text-center text-sm text-slate-400 shadow-sm">
        Nenhum veículo no histórico ainda.
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
              Saída
            </th>
            <th scope="col" className="px-4 py-3 text-right font-semibold">
              Valor cobrado
            </th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100">
          {veiculos.map((veiculo) => (
            <tr key={veiculo.id} className="transition-colors hover:bg-slate-50">
              <td className="px-4 py-3 font-mono text-base font-semibold tracking-wider text-slate-800">
                {veiculo.placa}
              </td>
              <td className="px-4 py-3 text-slate-600">{formatarDataHora(veiculo.horaEntrada)}</td>
              <td className="px-4 py-3 text-slate-600">
                {veiculo.horaSaida ? formatarDataHora(veiculo.horaSaida) : '—'}
              </td>
              <td className="px-4 py-3 text-right font-semibold tabular-nums text-slate-800">
                {veiculo.valorCobrado !== null ? formatarMoeda(veiculo.valorCobrado) : '—'}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
