using EstacionamentoDIO.Domain;
using EstacionamentoDIO.Domain.Entities;
using EstacionamentoDIO.Domain.Exceptions;
using EstacionamentoDIO.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace EstacionamentoDIO.Application;

public class EstacionamentoAppService : IEstacionamentoAppService
{
    private readonly IVeiculoRepository repositorio;
    private readonly IRelogio relogio;
    private readonly INotificadorEventos notificador;
    private readonly EstacionamentoOptions opcoes;

    public EstacionamentoAppService(
        IVeiculoRepository repositorio,
        IRelogio relogio,
        INotificadorEventos notificador,
        IOptions<EstacionamentoOptions> opcoes)
    {
        this.repositorio = repositorio;
        this.relogio = relogio;
        this.notificador = notificador;
        this.opcoes = opcoes.Value;
    }

    public async Task<VeiculoDto> RegistrarEntradaAsync(string placa, CancellationToken cancellationToken = default)
    {
        if (!Placas.EhValida(placa))
        {
            throw new PlacaInvalidaException(placa);
        }

        var placaNormalizada = Placas.Normalizar(placa);

        var existente = await repositorio.ObterEstacionadoPorPlacaAsync(placaNormalizada, cancellationToken);
        if (existente is not null)
        {
            throw new VeiculoJaEstacionadoException(placaNormalizada);
        }

        var vagasOcupadas = await repositorio.ContarEstacionadosAsync(cancellationToken);
        if (vagasOcupadas >= opcoes.VagasTotais)
        {
            throw new EstacionamentoLotadoException();
        }

        var veiculo = new Veiculo(placaNormalizada, relogio.AgoraUtc);
        await repositorio.AdicionarAsync(veiculo, cancellationToken);
        await repositorio.SalvarAlteracoesAsync(cancellationToken);

        var dto = ParaDto(veiculo);
        var status = await ObterStatusAsync(cancellationToken);
        await notificador.NotificarEntradaAsync(dto, status, cancellationToken);

        return dto;
    }

    public async Task<RegistrarSaidaResultDto> RegistrarSaidaAsync(string placa, FormaPagamento formaPagamento, CancellationToken cancellationToken = default)
    {
        var placaNormalizada = Placas.Normalizar(placa);
        var veiculo = await repositorio.ObterEstacionadoPorPlacaAsync(placaNormalizada, cancellationToken)
            ?? throw new VeiculoNaoEncontradoException(placaNormalizada);

        var horas = veiculo.RegistrarSaida(relogio.AgoraUtc, opcoes.PrecoInicial, opcoes.PrecoPorHora, formaPagamento);
        await repositorio.SalvarAlteracoesAsync(cancellationToken);

        var resultado = new RegistrarSaidaResultDto(ParaDto(veiculo), veiculo.ValorCobrado!.Value, horas, formaPagamento);
        var status = await ObterStatusAsync(cancellationToken);
        await notificador.NotificarSaidaAsync(resultado, status, cancellationToken);

        return resultado;
    }

    public async Task<List<VeiculoDto>> ListarEstacionadosAsync(CancellationToken cancellationToken = default)
    {
        var veiculos = await repositorio.ListarEstacionadosAsync(cancellationToken);
        return veiculos.Select(ParaDto).ToList();
    }

    public async Task<List<VeiculoDto>> ListarHistoricoAsync(CancellationToken cancellationToken = default)
    {
        var veiculos = await repositorio.ListarHistoricoAsync(cancellationToken);
        return veiculos.Select(ParaDto).ToList();
    }

    public async Task<StatusEstacionamentoDto> ObterStatusAsync(CancellationToken cancellationToken = default)
    {
        var ocupadas = await repositorio.ContarEstacionadosAsync(cancellationToken);
        return new StatusEstacionamentoDto(
            opcoes.VagasTotais,
            Math.Max(0, opcoes.VagasTotais - ocupadas),
            opcoes.PrecoInicial,
            opcoes.PrecoPorHora);
    }

    public async Task<RelatorioFaturamentoDto> ObterRelatorioFaturamentoAsync(
        DateTime? inicioUtc,
        DateTime? fimUtc,
        CancellationToken cancellationToken = default)
    {
        var agora = relogio.AgoraUtc;
        var inicio = inicioUtc ?? new DateTime(agora.Year, agora.Month, agora.Day, 0, 0, 0, DateTimeKind.Utc);
        var fim = fimUtc ?? inicio.AddDays(1);

        var saidas = await repositorio.ListarSaidasNoPeriodoAsync(inicio, fim, cancellationToken);

        var porFormaPagamento = saidas
            .GroupBy(v => v.FormaPagamento!.Value)
            .Select(grupo => new FaturamentoPorFormaPagamentoDto(
                grupo.Key,
                grupo.Count(),
                grupo.Sum(v => v.ValorCobrado!.Value)))
            .OrderByDescending(f => f.Total)
            .ToList();

        return new RelatorioFaturamentoDto(
            inicio,
            fim,
            saidas.Count,
            saidas.Sum(v => v.ValorCobrado!.Value),
            porFormaPagamento);
    }

    private static VeiculoDto ParaDto(Veiculo veiculo) => new(
        veiculo.Id,
        veiculo.Placa,
        veiculo.HoraEntrada,
        veiculo.HoraSaida,
        veiculo.ValorCobrado,
        veiculo.FormaPagamento);
}
