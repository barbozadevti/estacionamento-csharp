using EstacionamentoDIO.Domain.Entities;
using EstacionamentoDIO.Domain.Exceptions;

namespace EstacionamentoDIO.Domain.Tests;

public class VeiculoTests
{
    [Fact]
    public void Construtor_ComPlacaValida_DeveNormalizarENormalizarPlaca()
    {
        var entrada = new DateTime(2026, 1, 1, 10, 0, 0);

        var veiculo = new Veiculo(" abc1234 ", entrada);

        Assert.Equal("ABC1234", veiculo.Placa);
        Assert.Equal(entrada, veiculo.HoraEntrada);
        Assert.True(veiculo.EstaEstacionado);
    }

    [Fact]
    public void Construtor_ComPlacaInvalida_DeveLancarExcecao()
    {
        Assert.Throws<PlacaInvalidaException>(() => new Veiculo("XYZ", DateTime.UtcNow));
    }

    [Theory]
    [InlineData(0, 1)]   // 0 minutos ainda cobra 1 hora cheia
    [InlineData(59, 1)]  // 59 minutos ainda é 1 hora
    [InlineData(61, 2)]  // pouco mais de 1h já cobra 2 horas
    [InlineData(180, 3)] // exatas 3 horas
    [InlineData(181, 4)] // 3h01 vira 4 horas
    public void RegistrarSaida_DeveCobrarHoraCheiaArredondadaParaCima(int minutosEstacionado, int horasEsperadas)
    {
        var entrada = new DateTime(2026, 1, 1, 10, 0, 0);
        var veiculo = new Veiculo("ABC1234", entrada);
        var saida = entrada.AddMinutes(minutosEstacionado);

        var horas = veiculo.RegistrarSaida(saida, precoInicial: 5m, precoPorHora: 2m);

        Assert.Equal(horasEsperadas, horas);
        Assert.Equal(5m + 2m * horasEsperadas, veiculo.ValorCobrado);
        Assert.Equal(saida, veiculo.HoraSaida);
        Assert.False(veiculo.EstaEstacionado);
    }

    [Fact]
    public void RegistrarSaida_QuandoJaSaiu_DeveLancarExcecao()
    {
        var veiculo = new Veiculo("ABC1234", DateTime.UtcNow);
        veiculo.RegistrarSaida(DateTime.UtcNow.AddHours(1), 5m, 2m);

        Assert.Throws<InvalidOperationException>(() => veiculo.RegistrarSaida(DateTime.UtcNow.AddHours(2), 5m, 2m));
    }
}
