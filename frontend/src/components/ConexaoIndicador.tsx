import type { StatusConexao } from '../api/realtime'

const CONFIGURACOES: Record<StatusConexao, { texto: string; ponto: string; cor: string }> = {
  conectado: { texto: 'Ao vivo', ponto: 'bg-emerald-500', cor: 'text-emerald-700' },
  conectando: { texto: 'Conectando...', ponto: 'bg-amber-400 animate-pulse', cor: 'text-amber-700' },
  reconectando: { texto: 'Reconectando...', ponto: 'bg-amber-400 animate-pulse', cor: 'text-amber-700' },
  desconectado: { texto: 'Sem tempo real', ponto: 'bg-slate-400', cor: 'text-slate-500' },
}

export function ConexaoIndicador({ status }: { status: StatusConexao }) {
  const config = CONFIGURACOES[status]

  return (
    <span className={`inline-flex items-center gap-1.5 text-xs font-medium ${config.cor}`}>
      <span className={`h-1.5 w-1.5 rounded-full ${config.ponto}`} />
      {config.texto}
    </span>
  )
}
