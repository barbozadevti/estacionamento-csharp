import { useCallback, useEffect, useState } from 'react'
import { ApiError, buscarStatus, listarHistoricoVeiculos, listarVeiculosEstacionados, registrarEntrada, registrarSaida } from './api/client'
import { EntradaForm } from './components/EntradaForm'
import { ErroConexaoBanner } from './components/ErroConexaoBanner'
import { Header } from './components/Header'
import { HistoricoTable } from './components/HistoricoTable'
import { StatusCards } from './components/StatusCards'
import { Tabs, type Aba } from './components/Tabs'
import { Toast, type ToastData } from './components/Toast'
import { VeiculosEstacionadosTable } from './components/VeiculosEstacionadosTable'
import type { StatusEstacionamento, Veiculo } from './types/estacionamento'
import { formatarMoeda } from './utils/format'

const INTERVALO_POLL_MS = 9000

function mensagemDeErro(err: unknown, fallback: string): string {
  return err instanceof ApiError ? err.message : fallback
}

function App() {
  const [status, setStatus] = useState<StatusEstacionamento | null>(null)
  const [veiculosEstacionados, setVeiculosEstacionados] = useState<Veiculo[]>([])
  const [historico, setHistorico] = useState<Veiculo[]>([])
  const [abaAtiva, setAbaAtiva] = useState<Aba>('estacionados')

  const [carregandoInicial, setCarregandoInicial] = useState(true)
  const [carregandoHistorico, setCarregandoHistorico] = useState(false)
  const [erroConexao, setErroConexao] = useState<string | null>(null)
  const [placasProcessando, setPlacasProcessando] = useState<Set<string>>(new Set())
  const [toast, setToast] = useState<ToastData | null>(null)

  const carregarPrincipal = useCallback(async () => {
    try {
      const [statusResp, estacionadosResp] = await Promise.all([
        buscarStatus(),
        listarVeiculosEstacionados(),
      ])
      setStatus(statusResp)
      setVeiculosEstacionados(estacionadosResp)
      setErroConexao(null)
    } catch (err) {
      setErroConexao(mensagemDeErro(err, 'Erro desconhecido ao carregar dados.'))
    } finally {
      setCarregandoInicial(false)
    }
  }, [])

  const carregarHistorico = useCallback(async () => {
    setCarregandoHistorico(true)
    try {
      const resp = await listarHistoricoVeiculos()
      setHistorico(resp)
      setErroConexao(null)
    } catch (err) {
      setErroConexao(mensagemDeErro(err, 'Erro desconhecido ao carregar histórico.'))
    } finally {
      setCarregandoHistorico(false)
    }
  }, [])

  // Carregamento inicial dos dados principais.
  useEffect(() => {
    carregarPrincipal()
  }, [carregarPrincipal])

  // Carrega o histórico assim que a aba correspondente é aberta.
  useEffect(() => {
    if (abaAtiva === 'historico') {
      carregarHistorico()
    }
  }, [abaAtiva, carregarHistorico])

  // Poll periódico para manter os dados sincronizados com o servidor.
  useEffect(() => {
    const id = window.setInterval(() => {
      carregarPrincipal()
      if (abaAtiva === 'historico') {
        carregarHistorico()
      }
    }, INTERVALO_POLL_MS)
    return () => window.clearInterval(id)
  }, [carregarPrincipal, carregarHistorico, abaAtiva])

  async function handleRegistrarEntrada(placa: string) {
    try {
      const veiculo = await registrarEntrada(placa)
      setVeiculosEstacionados((prev) => [veiculo, ...prev])
      setStatus((prev) =>
        prev ? { ...prev, vagasDisponiveis: Math.max(0, prev.vagasDisponiveis - 1) } : prev,
      )
      setToast({ tipo: 'sucesso', mensagem: `Entrada registrada para ${veiculo.placa}.` })
    } catch (err) {
      setToast({ tipo: 'erro', mensagem: mensagemDeErro(err, 'Erro ao registrar entrada.') })
    }
  }

  async function handleRegistrarSaida(placa: string) {
    setPlacasProcessando((prev) => new Set(prev).add(placa))
    try {
      const resposta = await registrarSaida(placa)
      setVeiculosEstacionados((prev) => prev.filter((v) => v.placa !== placa))
      setHistorico((prev) => [resposta.veiculo, ...prev])
      setStatus((prev) =>
        prev
          ? { ...prev, vagasDisponiveis: Math.min(prev.vagasTotais, prev.vagasDisponiveis + 1) }
          : prev,
      )
      setToast({
        tipo: 'sucesso',
        mensagem: `Saída registrada para ${placa}. Valor cobrado: ${formatarMoeda(
          resposta.valorCobrado,
        )} (${resposta.horas}h de permanência).`,
      })
    } catch (err) {
      setToast({ tipo: 'erro', mensagem: mensagemDeErro(err, 'Erro ao registrar saída.') })
    } finally {
      setPlacasProcessando((prev) => {
        const next = new Set(prev)
        next.delete(placa)
        return next
      })
    }
  }

  return (
    <div className="min-h-screen bg-slate-100">
      <Header />

      <main className="mx-auto flex max-w-5xl flex-col gap-6 px-4 py-6 sm:px-6">
        {erroConexao && (
          <ErroConexaoBanner mensagem={erroConexao} onTentarNovamente={carregarPrincipal} />
        )}

        <StatusCards status={status} carregando={carregandoInicial} />

        <EntradaForm onRegistrar={handleRegistrarEntrada} desabilitado={carregandoInicial} />

        <Tabs
          abaAtiva={abaAtiva}
          onMudarAba={setAbaAtiva}
          totalEstacionados={veiculosEstacionados.length}
        />

        {abaAtiva === 'estacionados' ? (
          <VeiculosEstacionadosTable
            veiculos={veiculosEstacionados}
            onRegistrarSaida={handleRegistrarSaida}
            placasProcessando={placasProcessando}
            carregando={carregandoInicial}
          />
        ) : (
          <HistoricoTable
            veiculos={historico}
            carregando={carregandoHistorico && historico.length === 0}
          />
        )}
      </main>

      <Toast toast={toast} onFechar={() => setToast(null)} />
    </div>
  )
}

export default App
