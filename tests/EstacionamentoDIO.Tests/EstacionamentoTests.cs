using EstacionamentoDIO.Core;

namespace EstacionamentoDIO.Tests;

public class EstacionamentoTests
{
    [Fact]
    public void AdicionarVeiculo_ComPlacaValida_DeveAdicionar()
    {
        var estacionamento = new Estacionamento(precoInicial: 5m, precoPorHora: 2m, vagasTotais: 10);

        var adicionou = estacionamento.AdicionarVeiculo("abc1234", out var mensagem);

        Assert.True(adicionou);
        Assert.Equal("Veículo adicionado com sucesso!", mensagem);
        Assert.Single(estacionamento.ListarVeiculos());
        Assert.Equal("ABC1234", estacionamento.ListarVeiculos()[0].Placa);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void AdicionarVeiculo_ComPlacaInvalida_NaoDeveAdicionar(string? placa)
    {
        var estacionamento = new Estacionamento(precoInicial: 5m, precoPorHora: 2m, vagasTotais: 10);

        var adicionou = estacionamento.AdicionarVeiculo(placa, out var mensagem);

        Assert.False(adicionou);
        Assert.Equal("Placa inválida.", mensagem);
    }

    [Fact]
    public void AdicionarVeiculo_JaEstacionado_NaoDeveDuplicar()
    {
        var estacionamento = new Estacionamento(precoInicial: 5m, precoPorHora: 2m, vagasTotais: 10);
        estacionamento.AdicionarVeiculo("ABC1234", out _);

        var adicionou = estacionamento.AdicionarVeiculo("abc1234", out var mensagem);

        Assert.False(adicionou);
        Assert.Equal("Esse veículo já está estacionado.", mensagem);
        Assert.Single(estacionamento.ListarVeiculos());
    }

    [Fact]
    public void AdicionarVeiculo_SemVagasDisponiveis_DeveRecusar()
    {
        var estacionamento = new Estacionamento(precoInicial: 5m, precoPorHora: 2m, vagasTotais: 1);
        estacionamento.AdicionarVeiculo("ABC1234", out _);

        var adicionou = estacionamento.AdicionarVeiculo("XYZ9876", out var mensagem);

        Assert.False(adicionou);
        Assert.Equal("Estacionamento lotado. Não há vagas disponíveis.", mensagem);
        Assert.Equal(0, estacionamento.VagasDisponiveis);
    }

    [Fact]
    public void RemoverVeiculo_NaoEstacionado_DeveRetornarErro()
    {
        var estacionamento = new Estacionamento(precoInicial: 5m, precoPorHora: 2m, vagasTotais: 10);

        var removeu = estacionamento.RemoverVeiculo("ABC1234", out var valor, out var mensagem);

        Assert.False(removeu);
        Assert.Equal(0, valor);
        Assert.Equal("Desculpe, esse veículo não está no nosso estacionamento.", mensagem);
    }

    [Fact]
    public void RemoverVeiculo_Estacionado_DeveCobrarPrecoInicialMaisHoras()
    {
        var agora = new DateTime(2026, 1, 1, 10, 0, 0);
        var estacionamento = new Estacionamento(
            precoInicial: 5m,
            precoPorHora: 2m,
            vagasTotais: 10,
            relogio: () => agora);

        estacionamento.AdicionarVeiculo("ABC1234", out _);
        agora = agora.AddHours(3).AddMinutes(30); // 3h30 -> cobra 4 horas (arredonda para cima)

        var removeu = estacionamento.RemoverVeiculo("ABC1234", out var valor, out var mensagem);

        Assert.True(removeu);
        Assert.Equal(5m + 2m * 4, valor);
        Assert.Contains("4 hora(s)", mensagem);
        Assert.Empty(estacionamento.ListarVeiculos());
    }

    [Fact]
    public void Persistencia_DeveSalvarERecarregarVeiculos()
    {
        var arquivo = Path.Combine(Path.GetTempPath(), $"estacionamento-teste-{Guid.NewGuid()}.json");
        try
        {
            var primeiraInstancia = new Estacionamento(precoInicial: 5m, precoPorHora: 2m, vagasTotais: 10, arquivoDados: arquivo);
            primeiraInstancia.AdicionarVeiculo("ABC1234", out _);

            var segundaInstancia = new Estacionamento(precoInicial: 5m, precoPorHora: 2m, vagasTotais: 10, arquivoDados: arquivo);

            Assert.Single(segundaInstancia.ListarVeiculos());
            Assert.Equal("ABC1234", segundaInstancia.ListarVeiculos()[0].Placa);
        }
        finally
        {
            if (File.Exists(arquivo))
            {
                File.Delete(arquivo);
            }
        }
    }
}
