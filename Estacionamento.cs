using System.Globalization;

namespace EstacionamentoDIO;

public class Estacionamento
{
    private readonly decimal precoInicial;
    private readonly decimal precoPorHora;
    private readonly List<string> veiculos = new();

    public Estacionamento(decimal precoInicial, decimal precoPorHora)
    {
        this.precoInicial = precoInicial;
        this.precoPorHora = precoPorHora;
    }

    public void AdicionarVeiculo()
    {
        Console.WriteLine("Digite a placa do veículo para estacionar:");
        var placa = Console.ReadLine()?.Trim().ToUpper();

        if (string.IsNullOrWhiteSpace(placa))
        {
            Console.WriteLine("Placa inválida. Operação cancelada.");
            return;
        }

        if (veiculos.Contains(placa))
        {
            Console.WriteLine("Esse veículo já está estacionado.");
            return;
        }

        veiculos.Add(placa);
        Console.WriteLine("Veículo adicionado com sucesso!");
    }

    public void RemoverVeiculo()
    {
        Console.WriteLine("Digite a placa do veículo para remover:");
        var placa = Console.ReadLine()?.Trim().ToUpper();

        if (placa is not null && veiculos.Contains(placa))
        {
            Console.WriteLine("Digite a quantidade de horas que o veículo permaneceu estacionado:");
            var entradaHoras = Console.ReadLine();

            if (!double.TryParse(entradaHoras, NumberStyles.Any, CultureInfo.InvariantCulture, out var horas) || horas < 0)
            {
                Console.WriteLine("Quantidade de horas inválida. Operação cancelada.");
                return;
            }

            var valorTotal = precoInicial + (precoPorHora * (decimal)horas);
            veiculos.Remove(placa);

            Console.WriteLine($"O valor total foi de: {valorTotal.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"))}");
        }
        else
        {
            Console.WriteLine("Desculpe, esse veículo não está no nosso estacionamento.");
        }
    }

    public void ListarVeiculos()
    {
        if (veiculos.Count > 0)
        {
            Console.WriteLine("Os veículos estacionados são:");
            foreach (var veiculo in veiculos)
            {
                Console.WriteLine($"- {veiculo}");
            }
        }
        else
        {
            Console.WriteLine("Não há veículos estacionados.");
        }
    }
}
