export function Header() {
  return (
    <header className="border-b border-slate-200 bg-white/80 backdrop-blur">
      <div className="mx-auto flex max-w-5xl items-center gap-3 px-4 py-5 sm:px-6">
        <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-blue-600 text-lg text-white shadow-sm">
          🅿️
        </div>
        <div>
          <h1 className="text-lg font-bold leading-tight text-slate-900 sm:text-xl">
            Sistema de Estacionamento
          </h1>
          <p className="text-xs text-slate-500">Painel operacional em tempo real</p>
        </div>
      </div>
    </header>
  )
}
