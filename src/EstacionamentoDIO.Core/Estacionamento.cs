using System.Globalization;
using System.Text.Json;

namespace EstacionamentoDIO.Core;

public class Estacionamento
{
    private readonly decimal precoInicial;
    private readonly decimal precoPorHora;
    private readonly int vagasTotais;
    private readonly string? arquivoDados;
    private readonly Func<DateTime> relogio;
    private List<Veiculo> veiculos = new();

    public Estacionamento(
        decimal precoInicial,
        decimal precoPorHora,
        int vagasTotais,
        string? arquivoDados = null,
        Func<DateTime>? relogio = null)
    {
        this.precoInicial = precoInicial;
        this.precoPorHora = precoPorHora;
        this.vagasTotais = vagasTotais;
        this.arquivoDados = arquivoDados;
        this.relogio = relogio ?? (() => DateTime.Now);
        CarregarDados();
    }

    public int VagasTotais => vagasTotais;

    public int VagasDisponiveis => vagasTotais - veiculos.Count;

    public bool AdicionarVeiculo(string? placaInformada, out string mensagem)
    {
        var placa = placaInformada?.Trim().ToUpper() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(placa))
        {
            mensagem = "Placa inválida.";
            return false;
        }

        if (veiculos.Any(v => v.Placa == placa))
        {
            mensagem = "Esse veículo já está estacionado.";
            return false;
        }

        if (VagasDisponiveis <= 0)
        {
            mensagem = "Estacionamento lotado. Não há vagas disponíveis.";
            return false;
        }

        veiculos.Add(new Veiculo { Placa = placa, HoraEntrada = relogio() });
        SalvarDados();
        mensagem = "Veículo adicionado com sucesso!";
        return true;
    }

    public bool RemoverVeiculo(string? placaInformada, out decimal valorTotal, out string mensagem)
    {
        var placa = placaInformada?.Trim().ToUpper() ?? string.Empty;
        var veiculo = veiculos.FirstOrDefault(v => v.Placa == placa);
        valorTotal = 0;

        if (veiculo is null)
        {
            mensagem = "Desculpe, esse veículo não está no nosso estacionamento.";
            return false;
        }

        var horas = Math.Max(1, (int)Math.Ceiling((relogio() - veiculo.HoraEntrada).TotalHours));
        valorTotal = precoInicial + (precoPorHora * horas);
        veiculos.Remove(veiculo);
        SalvarDados();
        mensagem = $"Veículo permaneceu {horas} hora(s). Valor total: {valorTotal.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"))}";
        return true;
    }

    public IReadOnlyList<Veiculo> ListarVeiculos() => veiculos.AsReadOnly();

    private void SalvarDados()
    {
        if (arquivoDados is null)
        {
            return;
        }

        var json = JsonSerializer.Serialize(veiculos, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(arquivoDados, json);
    }

    private void CarregarDados()
    {
        if (arquivoDados is null || !File.Exists(arquivoDados))
        {
            return;
        }

        var json = File.ReadAllText(arquivoDados);
        if (string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        veiculos = JsonSerializer.Deserialize<List<Veiculo>>(json) ?? new List<Veiculo>();
    }
}
