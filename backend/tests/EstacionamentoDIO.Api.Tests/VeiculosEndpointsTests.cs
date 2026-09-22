using System.Net;
using System.Net.Http.Json;
using EstacionamentoDIO.Application;

namespace EstacionamentoDIO.Api.Tests;

public class VeiculosEndpointsTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient cliente;

    public VeiculosEndpointsTests(ApiFactory fabrica)
    {
        cliente = fabrica.CreateClient();
    }

    [Fact]
    public async Task RegistrarEntrada_ComPlacaValida_DeveRetornar201()
    {
        var placa = PlacaUnica();

        var resposta = await cliente.PostAsJsonAsync("/api/veiculos/entrada", new { placa });

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        var veiculo = await resposta.Content.ReadFromJsonAsync<VeiculoDto>();
        Assert.Equal(placa, veiculo!.Placa);
    }

    [Fact]
    public async Task RegistrarEntrada_Duplicada_DeveRetornar409()
    {
        var placa = PlacaUnica();
        await cliente.PostAsJsonAsync("/api/veiculos/entrada", new { placa });

        var resposta = await cliente.PostAsJsonAsync("/api/veiculos/entrada", new { placa });

        Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
        var corpo = await resposta.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.True(corpo!.ContainsKey("message"));
    }

    [Fact]
    public async Task RegistrarEntrada_ComPlacaInvalida_DeveRetornar422()
    {
        var resposta = await cliente.PostAsJsonAsync("/api/veiculos/entrada", new { placa = "XX" });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, resposta.StatusCode);
    }

    [Fact]
    public async Task RegistrarSaida_VeiculoInexistente_DeveRetornar404()
    {
        var resposta = await cliente.PostAsync("/api/veiculos/NUNCA99/saida", content: null);

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task FluxoCompleto_EntradaListagemESaida_DeveFuncionar()
    {
        var placa = PlacaUnica();

        await cliente.PostAsJsonAsync("/api/veiculos/entrada", new { placa });

        var listaEstacionados = await cliente.GetFromJsonAsync<List<VeiculoDto>>("/api/veiculos?status=estacionados");
        Assert.Contains(listaEstacionados!, v => v.Placa == placa);

        var respostaSaida = await cliente.PostAsync($"/api/veiculos/{placa}/saida", content: null);
        Assert.Equal(HttpStatusCode.OK, respostaSaida.StatusCode);

        var resultado = await respostaSaida.Content.ReadFromJsonAsync<RegistrarSaidaResultDto>();
        Assert.True(resultado!.ValorCobrado > 0);

        var historico = await cliente.GetFromJsonAsync<List<VeiculoDto>>("/api/veiculos?status=historico");
        Assert.Contains(historico!, v => v.Placa == placa);
    }

    [Fact]
    public async Task ObterStatus_DeveRetornarVagasEPrecos()
    {
        var status = await cliente.GetFromJsonAsync<StatusEstacionamentoDto>("/api/estacionamento/status");

        Assert.NotNull(status);
        Assert.True(status!.VagasTotais > 0);
    }

    private static string PlacaUnica() => $"TST{Random.Shared.Next(1000, 9999)}";
}
