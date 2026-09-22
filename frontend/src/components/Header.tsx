import type { Estabelecimento } from '../types/estacionamento'

interface HeaderProps {
  estabelecimento: Estabelecimento | null
}

export function Header({ estabelecimento }: HeaderProps) {
  return (
    <header className="border-b border-slate-200 bg-white/80 backdrop-blur">
      <div className="mx-auto flex max-w-5xl items-center gap-3 px-4 py-5 sm:px-6">
        <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-blue-600 text-lg text-white shadow-sm">
          🅿️
        </div>
        <div className="min-w-0">
          <h1 className="truncate text-lg font-bold leading-tight text-slate-900 sm:text-xl">
            {estabelecimento?.nome ?? 'Sistema de Estacionamento'}
          </h1>
          <p className="text-xs text-slate-500">
            Estacionamento · Painel operacional em tempo real
            {estabelecimento?.cnpj && (
              <>
                {' '}
                <span className="text-slate-400">· CNPJ {estabelecimento.cnpj}</span>
              </>
            )}
          </p>
        </div>
      </div>
    </header>
  )
}
