import { useState } from 'react'
import { FORMAS_PAGAMENTO, type Estabelecimento, type FormaPagamento } from '../types/estacionamento'

interface PagamentoModalProps {
  placa: string
  processando: boolean
  estabelecimento: Estabelecimento | null
  onConfirmar: (forma: FormaPagamento) => void
  onFechar: () => void
}

export function PagamentoModal({
  placa,
  processando,
  estabelecimento,
  onConfirmar,
  onFechar,
}: PagamentoModalProps) {
  const [formaSelecionada, setFormaSelecionada] = useState<FormaPagamento | null>(null)

  function handleSelecionar(forma: FormaPagamento) {
    if (forma === 'Pix') {
      setFormaSelecionada('Pix')
      return
    }
    onConfirmar(forma)
  }

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/50 p-4"
      onClick={processando ? undefined : onFechar}
    >
      <div
        className="w-full max-w-sm rounded-2xl bg-white p-6 shadow-xl"
        onClick={(event) => event.stopPropagation()}
      >
        <h2 className="text-lg font-semibold text-slate-900">
          Pagamento — <span className="font-mono tracking-wider">{placa}</span>
        </h2>
        <p className="mt-1 text-sm text-slate-500">
          Escolha a forma de pagamento para liberar a saída.
        </p>

        {formaSelecionada === 'Pix' ? (
          <div className="mt-4 flex flex-col items-center gap-3 rounded-xl border border-slate-200 bg-slate-50 p-4">
            <QrCodeSimulado />
            {estabelecimento && (
              <p className="text-center text-xs font-medium text-slate-600">
                Recebedor: {estabelecimento.nome}
                <br />
                <span className="font-normal text-slate-400">CNPJ {estabelecimento.cnpj}</span>
              </p>
            )}
            <p className="text-center text-xs text-slate-500">
              QR Code Pix simulado — projeto de demonstração, nenhuma cobrança real é feita.
            </p>
            <button
              type="button"
              disabled={processando}
              onClick={() => onConfirmar('Pix')}
              className="w-full rounded-lg bg-emerald-600 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-emerald-700 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {processando ? 'Confirmando...' : 'Já paguei'}
            </button>
            <button
              type="button"
              disabled={processando}
              onClick={() => setFormaSelecionada(null)}
              className="text-xs text-slate-400 hover:text-slate-600 disabled:opacity-50"
            >
              Escolher outra forma de pagamento
            </button>
          </div>
        ) : (
          <div className="mt-4 grid grid-cols-1 gap-2">
            {FORMAS_PAGAMENTO.map((forma) => (
              <button
                key={forma.valor}
                type="button"
                disabled={processando}
                onClick={() => handleSelecionar(forma.valor)}
                className="flex items-center justify-between rounded-lg border border-slate-200 px-4 py-2.5 text-left text-sm font-medium text-slate-700 transition hover:border-blue-400 hover:bg-blue-50 disabled:cursor-not-allowed disabled:opacity-50"
              >
                {forma.label}
                {processando && (
                  <span className="h-3 w-3 animate-spin rounded-full border-2 border-slate-300 border-t-slate-600" />
                )}
              </button>
            ))}
          </div>
        )}

        <button
          type="button"
          disabled={processando}
          onClick={onFechar}
          className="mt-4 w-full text-center text-xs text-slate-400 hover:text-slate-600 disabled:opacity-50"
        >
          Cancelar
        </button>
      </div>
    </div>
  )
}

/**
 * Padrão fixo só para parecer visualmente um QR code — não é um QR real,
 * é uma simulação para fins de demonstração do fluxo de pagamento.
 */
function QrCodeSimulado() {
  const tamanho = 9
  const celulas = Array.from({ length: tamanho * tamanho }, (_, i) => (i * 37) % 7 < 3)

  return (
    <svg
      viewBox={`0 0 ${tamanho} ${tamanho}`}
      className="h-32 w-32 rounded-lg border border-slate-200 bg-white p-1"
      role="img"
      aria-label="QR Code Pix simulado"
    >
      {celulas.map((preenchida, indice) =>
        preenchida ? (
          <rect
            key={indice}
            x={indice % tamanho}
            y={Math.floor(indice / tamanho)}
            width={1}
            height={1}
            fill="#0f172a"
          />
        ) : null,
      )}
    </svg>
  )
}
