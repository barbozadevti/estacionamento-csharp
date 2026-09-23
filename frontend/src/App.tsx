import { useCallback, useEffect, useState } from 'react'
import {
  ApiError,
  buscarEstabelecimento,
  buscarStatus,
  listarHistoricoVeiculos,
  listarVeiculosEstacionados,
  registrarEntrada,
  registrarSaida,
} from './api/client'
import { conectarTempoReal, type StatusConexao } from './api/realtime'
import { ConexaoIndicador } from './components/ConexaoIndicador'
import { EntradaForm } from './components/EntradaForm'
import { ErroConexaoBanner } from './components/ErroConexaoBanner'
import { Header } from './components/Header'
import { HistoricoTable } from './components/HistoricoTable'
import { PagamentoModal } from './components/PagamentoModal'
import { RelatorioPanel } from './components/RelatorioPanel'
import { StatusCards } from './components/StatusCards'
import { Tabs, type Aba } from './components/Tabs'
import { Toast, type ToastData } from './components/Toast'
import { VeiculosEstacionadosTable } from './components/VeiculosEstacionadosTable'
import type { Estabelecimento, FormaPagamento, StatusEstacionamento, Veiculo } from './types/estacionamento'
import { rotuloFormaPagamento } from './types/estacionamento'
import { formatarMoeda } from './utils/format'

// O SignalR mantém tudo sincronizado em tempo real; o poll fica só como
// rede de segurança caso a conexão em tempo real caia silenciosamente.
const INTERVALO_POLL_MS = 30000

function mensagemDeErro(err: unknown, fallback: string): string {
  return err instanceof ApiError ? err.message : fallback
}

function App() {
  const [estabelecimento, setEstabelecimento] = useState<Estabelecimento | null>(null)
  const [status, setStatus] = useState<StatusEstacionamento | null>(null)
  const [veiculosEstacionados, setVeiculosEstacionados] = useState<Veiculo[]>([])
  const [historico, setHistorico] = useState<Veiculo[]>([])
  const [abaAtiva, setAbaAtiva] = useState<Aba>('estacionados')

  const [carregandoInicial, setCarregandoInicial] = useState(true)
  const [carregandoHistorico, setCarregandoHistorico] = useState(false)
  const [erroConexao, setErroConexao] = useState<string | null>(null)
  const [placasProcessando, setPlacasProcessando] = useState<Set<string>>(new Set())
  const [toast, setToast] = useState<ToastData | null>(null)
  const [placaEmPagamento, setPlacaEmPagamento] = useState<string | null>(null)
  const [statusConexao, setStatusConexao] = useState<StatusConexao>('conectando')

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

  // Dados do estabelecimento raramente mudam; busca uma vez só, sem entrar no poll.
  useEffect(() => {
    buscarEstabelecimento()
      .then(setEstabelecimento)
      .catch(() => {
        // Falha silenciosa: o cabeçalho cai para o nome genérico se a API não responder.
      })
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

  // Poll periódico, como rede de segurança caso o tempo real caia silenciosamente.
  useEffect(() => {
    const id = window.setInterval(() => {
      carregarPrincipal()
      if (abaAtiva === 'historico') {
        carregarHistorico()
      }
    }, INTERVALO_POLL_MS)
    return () => window.clearInterval(id)
  }, [carregarPrincipal, carregarHistorico, abaAtiva])

  // Conexão em tempo real: outro operador registrando entrada/saída aparece
  // instantaneamente aqui, sem esperar o próximo poll.
  useEffect(() => {
    const desconectar = conectarTempoReal({
      onStatusConexaoMudou: setStatusConexao,
      onVeiculoEntrou: (veiculo, statusAtualizado) => {
        setStatus(statusAtualizado)
        setVeiculosEstacionados((prev) =>
          prev.some((v) => v.id === veiculo.id) ? prev : [veiculo, ...prev],
        )
      },
      onVeiculoSaiu: (resultado, statusAtualizado) => {
        setStatus(statusAtualizado)
        setVeiculosEstacionados((prev) => prev.filter((v) => v.id !== resultado.veiculo.id))
        setHistorico((prev) =>
          prev.some((v) => v.id === resultado.veiculo.id) ? prev : [resultado.veiculo, ...prev],
        )
      },
    })

    return desconectar
  }, [])

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

  async function handleConfirmarPagamento(placa: string, formaPagamento: FormaPagamento) {
    setPlacasProcessando((prev) => new Set(prev).add(placa))
    try {
      const resposta = await registrarSaida(placa, formaPagamento)
      setVeiculosEstacionados((prev) => prev.filter((v) => v.placa !== placa))
      setHistorico((prev) => [resposta.veiculo, ...prev])
      setStatus((prev) =>
        prev
          ? { ...prev, vagasDisponiveis: Math.min(prev.vagasTotais, prev.vagasDisponiveis + 1) }
          : prev,
      )
      setToast({
        tipo: 'sucesso',
        mensagem: `Saída registrada para ${placa} via ${rotuloFormaPagamento(formaPagamento)}. Valor cobrado: ${formatarMoeda(
          resposta.valorCobrado,
        )} (${resposta.horas}h de permanência).`,
      })
      setPlacaEmPagamento(null)
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
      <Header estabelecimento={estabelecimento} />

      <main className="mx-auto flex max-w-5xl flex-col gap-6 px-4 py-6 sm:px-6">
        {erroConexao && (
          <ErroConexaoBanner mensagem={erroConexao} onTentarNovamente={carregarPrincipal} />
        )}

        <StatusCards status={status} carregando={carregandoInicial} />

        <EntradaForm onRegistrar={handleRegistrarEntrada} desabilitado={carregandoInicial} />

        <div className="flex flex-wrap items-center justify-between gap-2">
          <Tabs
            abaAtiva={abaAtiva}
            onMudarAba={setAbaAtiva}
            totalEstacionados={veiculosEstacionados.length}
          />
          <ConexaoIndicador status={statusConexao} />
        </div>

        {abaAtiva === 'estacionados' && (
          <VeiculosEstacionadosTable
            veiculos={veiculosEstacionados}
            onAbrirPagamento={setPlacaEmPagamento}
            placasProcessando={placasProcessando}
            carregando={carregandoInicial}
          />
        )}
        {abaAtiva === 'historico' && (
          <HistoricoTable
            veiculos={historico}
            carregando={carregandoHistorico && historico.length === 0}
          />
        )}
        {abaAtiva === 'relatorio' && <RelatorioPanel />}
      </main>

      {placaEmPagamento && (
        <PagamentoModal
          placa={placaEmPagamento}
          processando={placasProcessando.has(placaEmPagamento)}
          estabelecimento={estabelecimento}
          onConfirmar={(forma) => handleConfirmarPagamento(placaEmPagamento, forma)}
          onFechar={() => setPlacaEmPagamento(null)}
        />
      )}

      <Toast toast={toast} onFechar={() => setToast(null)} />
    </div>
  )
}

export default App
