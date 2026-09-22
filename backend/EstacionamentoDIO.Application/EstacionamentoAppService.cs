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
    private readonly EstacionamentoOptions opcoes;

    public EstacionamentoAppService(
        IVeiculoRepository repositorio,
        IRelogio relogio,
        IOptions<EstacionamentoOptions> opcoes)
    {
        this.repositorio = repositorio;
        this.relogio = relogio;
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

        return ParaDto(veiculo);
    }

    public async Task<RegistrarSaidaResultDto> RegistrarSaidaAsync(string placa, CancellationToken cancellationToken = default)
    {
        var placaNormalizada = Placas.Normalizar(placa);
        var veiculo = await repositorio.ObterEstacionadoPorPlacaAsync(placaNormalizada, cancellationToken)
            ?? throw new VeiculoNaoEncontradoException(placaNormalizada);

        var horas = veiculo.RegistrarSaida(relogio.AgoraUtc, opcoes.PrecoInicial, opcoes.PrecoPorHora);
        await repositorio.SalvarAlteracoesAsync(cancellationToken);

        return new RegistrarSaidaResultDto(ParaDto(veiculo), veiculo.ValorCobrado!.Value, horas);
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

    private static VeiculoDto ParaDto(Veiculo veiculo) => new(
        veiculo.Id,
        veiculo.Placa,
        veiculo.HoraEntrada,
        veiculo.HoraSaida,
        veiculo.ValorCobrado);
}
