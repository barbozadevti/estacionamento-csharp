interface ErroConexaoBannerProps {
  mensagem: string
  onTentarNovamente: () => void
}

export function ErroConexaoBanner({ mensagem, onTentarNovamente }: ErroConexaoBannerProps) {
  return (
    <div className="flex flex-col gap-3 rounded-2xl border border-red-200 bg-red-50 p-5 shadow-sm sm:flex-row sm:items-center sm:justify-between">
      <div className="flex items-start gap-3">
        <span className="text-xl leading-none">⚠</span>
        <div>
          <p className="text-sm font-semibold text-red-800">Falha de conexão com o servidor</p>
          <p className="text-sm text-red-700">{mensagem}</p>
        </div>
      </div>
      <button
        type="button"
        onClick={onTentarNovamente}
        className="shrink-0 rounded-lg border border-red-300 bg-white px-4 py-2 text-sm font-semibold text-red-700 shadow-sm transition hover:bg-red-100"
      >
        Tentar novamente
      </button>
    </div>
  )
}
