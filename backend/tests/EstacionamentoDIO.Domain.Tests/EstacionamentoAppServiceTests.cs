using EstacionamentoDIO.Application;
using EstacionamentoDIO.Domain.Entities;
using EstacionamentoDIO.Domain.Exceptions;
using EstacionamentoDIO.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace EstacionamentoDIO.Domain.Tests;

public class EstacionamentoAppServiceTests
{
    private static EstacionamentoAppService CriarServico(
        FakeVeiculoRepository repositorio,
        FakeRelogio relogio,
        int vagasTotais = 2)
    {
        var opcoes = Options.Create(new EstacionamentoOptions
        {
            VagasTotais = vagasTotais,
            PrecoInicial = 5m,
            PrecoPorHora = 2m,
        });

        return new EstacionamentoAppService(repositorio, relogio, opcoes);
    }

    [Fact]
    public async Task RegistrarEntradaAsync_ComPlacaNova_DeveAdicionar()
    {
        var repositorio = new FakeVeiculoRepository();
        var relogio = new FakeRelogio(new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc));
        var servico = CriarServico(repositorio, relogio);

        var dto = await servico.RegistrarEntradaAsync("abc1234");

        Assert.Equal("ABC1234", dto.Placa);
        Assert.Single(repositorio.Veiculos);
    }

    [Fact]
    public async Task RegistrarEntradaAsync_ComVeiculoJaEstacionado_DeveLancarExcecao()
    {
        var repositorio = new FakeVeiculoRepository();
        var relogio = new FakeRelogio(DateTime.UtcNow);
        var servico = CriarServico(repositorio, relogio);
        await servico.RegistrarEntradaAsync("ABC1234");

        await Assert.ThrowsAsync<VeiculoJaEstacionadoException>(() => servico.RegistrarEntradaAsync("ABC1234"));
    }

    [Fact]
    public async Task RegistrarEntradaAsync_SemVagas_DeveLancarExcecao()
    {
        var repositorio = new FakeVeiculoRepository();
        var relogio = new FakeRelogio(DateTime.UtcNow);
        var servico = CriarServico(repositorio, relogio, vagasTotais: 1);
        await servico.RegistrarEntradaAsync("ABC1234");

        await Assert.ThrowsAsync<EstacionamentoLotadoException>(() => servico.RegistrarEntradaAsync("XYZ9876"));
    }

    [Fact]
    public async Task RegistrarSaidaAsync_VeiculoNaoEstacionado_DeveLancarExcecao()
    {
        var repositorio = new FakeVeiculoRepository();
        var relogio = new FakeRelogio(DateTime.UtcNow);
        var servico = CriarServico(repositorio, relogio);

        await Assert.ThrowsAsync<VeiculoNaoEncontradoException>(() => servico.RegistrarSaidaAsync("ABC1234", FormaPagamento.Pix));
    }

    [Fact]
    public async Task RegistrarSaidaAsync_DeveCalcularValorELiberarVaga()
    {
        var repositorio = new FakeVeiculoRepository();
        var relogio = new FakeRelogio(new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc));
        var servico = CriarServico(repositorio, relogio);
        await servico.RegistrarEntradaAsync("ABC1234");

        relogio.Avancar(TimeSpan.FromHours(2.5));
        var resultado = await servico.RegistrarSaidaAsync("abc1234", FormaPagamento.Pix);

        Assert.Equal(3, resultado.Horas);
        Assert.Equal(5m + 2m * 3, resultado.ValorCobrado);
        Assert.Equal(FormaPagamento.Pix, resultado.FormaPagamento);
        Assert.Equal(FormaPagamento.Pix, resultado.Veiculo.FormaPagamento);

        var status = await servico.ObterStatusAsync();
        Assert.Equal(status.VagasTotais, status.VagasDisponiveis);
    }

    private class FakeRelogio : IRelogio
    {
        public FakeRelogio(DateTime inicial) => AgoraUtc = inicial;
        public DateTime AgoraUtc { get; private set; }
        public void Avancar(TimeSpan tempo) => AgoraUtc = AgoraUtc.Add(tempo);
    }

    private class FakeVeiculoRepository : IVeiculoRepository
    {
        public List<Veiculo> Veiculos { get; } = new();

        public Task<Veiculo?> ObterEstacionadoPorPlacaAsync(string placa, CancellationToken cancellationToken = default) =>
            Task.FromResult(Veiculos.FirstOrDefault(v => v.Placa == placa && v.EstaEstacionado));

        public Task<List<Veiculo>> ListarEstacionadosAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Veiculos.Where(v => v.EstaEstacionado).ToList());

        public Task<List<Veiculo>> ListarHistoricoAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Veiculos.Where(v => !v.EstaEstacionado).ToList());

        public Task<int> ContarEstacionadosAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Veiculos.Count(v => v.EstaEstacionado));

        public Task AdicionarAsync(Veiculo veiculo, CancellationToken cancellationToken = default)
        {
            Veiculos.Add(veiculo);
            return Task.CompletedTask;
        }

        public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
