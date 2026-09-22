import { useEffect } from 'react'

export interface ToastData {
  tipo: 'sucesso' | 'erro'
  mensagem: string
}

interface ToastProps {
  toast: ToastData | null
  onFechar: () => void
  duracaoMs?: number
}

export function Toast({ toast, onFechar, duracaoMs = 5000 }: ToastProps) {
  useEffect(() => {
    if (!toast) return
    const id = window.setTimeout(onFechar, duracaoMs)
    return () => window.clearTimeout(id)
  }, [toast, onFechar, duracaoMs])

  if (!toast) return null

  const estilos =
    toast.tipo === 'sucesso'
      ? 'bg-emerald-50 border-emerald-300 text-emerald-800'
      : 'bg-red-50 border-red-300 text-red-800'

  return (
    <div
      role="status"
      className={`fixed bottom-4 right-4 left-4 z-50 mx-auto max-w-sm rounded-xl border px-4 py-3 shadow-lg backdrop-blur sm:left-auto ${estilos}`}
    >
      <div className="flex items-start gap-3">
        <span className="mt-0.5 text-lg leading-none">
          {toast.tipo === 'sucesso' ? '✓' : '⚠'}
        </span>
        <p className="flex-1 text-sm font-medium">{toast.mensagem}</p>
        <button
          type="button"
          onClick={onFechar}
          className="text-sm font-semibold opacity-60 transition hover:opacity-100"
          aria-label="Fechar notificação"
        >
          ×
        </button>
      </div>
    </div>
  )
}
