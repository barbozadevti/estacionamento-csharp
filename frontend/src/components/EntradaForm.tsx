import { useState } from 'react'
import type { FormEvent } from 'react'

interface EntradaFormProps {
  onRegistrar: (placa: string) => Promise<void>
  desabilitado: boolean
}

export function EntradaForm({ onRegistrar, desabilitado }: EntradaFormProps) {
  const [placa, setPlaca] = useState('')
  const [enviando, setEnviando] = useState(false)
  const [erroLocal, setErroLocal] = useState<string | null>(null)

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setErroLocal(null)

    const placaNormalizada = placa.trim().toUpperCase()
    if (placaNormalizada === '') {
      setErroLocal('Informe a placa do veículo.')
      return
    }

    setEnviando(true)
    try {
      await onRegistrar(placaNormalizada)
      setPlaca('')
    } finally {
      setEnviando(false)
    }
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-5 shadow-sm sm:flex-row sm:items-end"
    >
      <div className="flex-1">
        <label htmlFor="placa" className="mb-1 block text-sm font-medium text-slate-700">
          Placa do veículo
        </label>
        <input
          id="placa"
          type="text"
          value={placa}
          onChange={(e) => {
            setPlaca(e.target.value.toUpperCase())
            if (erroLocal) setErroLocal(null)
          }}
          placeholder="ABC1234"
          maxLength={8}
          autoComplete="off"
          spellCheck={false}
          className="w-full rounded-lg border border-slate-300 bg-slate-50 px-3 py-2 font-mono text-lg uppercase tracking-widest text-slate-900 outline-none transition focus:border-blue-500 focus:bg-white focus:ring-2 focus:ring-blue-100"
        />
        {erroLocal && <p className="mt-1 text-sm text-red-600">{erroLocal}</p>}
      </div>
      <button
        type="submit"
        disabled={desabilitado || enviando}
        className="inline-flex items-center justify-center gap-2 rounded-lg bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white shadow-sm transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-slate-300"
      >
        {enviando && (
          <span className="h-4 w-4 animate-spin rounded-full border-2 border-white/40 border-t-white" />
        )}
        {enviando ? 'Registrando...' : 'Registrar entrada'}
      </button>
    </form>
  )
}
