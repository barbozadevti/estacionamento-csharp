export type Aba = 'estacionados' | 'historico'

interface TabsProps {
  abaAtiva: Aba
  onMudarAba: (aba: Aba) => void
  totalEstacionados: number
}

export function Tabs({ abaAtiva, onMudarAba, totalEstacionados }: TabsProps) {
  const abas: { id: Aba; label: string; contagem?: number }[] = [
    { id: 'estacionados', label: 'Estacionados', contagem: totalEstacionados },
    { id: 'historico', label: 'Histórico' },
  ]

  return (
    <div className="inline-flex gap-1 rounded-xl bg-slate-200/70 p-1">
      {abas.map((aba) => {
        const ativa = aba.id === abaAtiva
        return (
          <button
            key={aba.id}
            type="button"
            onClick={() => onMudarAba(aba.id)}
            className={`rounded-lg px-4 py-2 text-sm font-semibold transition ${
              ativa
                ? 'bg-white text-blue-700 shadow-sm'
                : 'text-slate-600 hover:text-slate-900'
            }`}
          >
            {aba.label}
            {typeof aba.contagem === 'number' && (
              <span
                className={`ml-2 rounded-full px-1.5 py-0.5 text-xs ${
                  ativa ? 'bg-blue-100 text-blue-700' : 'bg-slate-300/70 text-slate-600'
                }`}
              >
                {aba.contagem}
              </span>
            )}
          </button>
        )
      })}
    </div>
  )
}
