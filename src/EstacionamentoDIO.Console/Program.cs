using EstacionamentoDIO.Core;

var estacionamento = new Estacionamento(
    precoInicial: 5.00m,
    precoPorHora: 2.00m,
    vagasTotais: 10,
    arquivoDados: "estacionamento.json");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Sistema de Estacionamento ===");
    Console.WriteLine($"Vagas disponíveis: {estacionamento.VagasDisponiveis}/{estacionamento.VagasTotais}");
    Console.WriteLine("Escolha a opção desejada:");
    Console.WriteLine("1 - Adicionar veículo");
    Console.WriteLine("2 - Remover veículo");
    Console.WriteLine("3 - Listar veículos");
    Console.WriteLine("4 - Sair");
    Console.Write("Opção: ");

    var opcao = Console.ReadLine();
    Console.WriteLine();

    switch (opcao)
    {
        case "1":
            Console.WriteLine("Digite a placa do veículo para estacionar:");
            var placaEntrada = Console.ReadLine();
            estacionamento.AdicionarVeiculo(placaEntrada, out var mensagemEntrada);
            Console.WriteLine(mensagemEntrada);
            break;

        case "2":
            Console.WriteLine("Digite a placa do veículo para remover:");
            var placaSaida = Console.ReadLine();
            estacionamento.RemoverVeiculo(placaSaida, out _, out var mensagemSaida);
            Console.WriteLine(mensagemSaida);
            break;

        case "3":
            var veiculos = estacionamento.ListarVeiculos();
            if (veiculos.Count > 0)
            {
                Console.WriteLine("Os veículos estacionados são:");
                foreach (var veiculo in veiculos)
                {
                    Console.WriteLine($"- {veiculo.Placa} (entrada: {veiculo.HoraEntrada:dd/MM/yyyy HH:mm:ss})");
                }
            }
            else
            {
                Console.WriteLine("Não há veículos estacionados.");
            }
            break;

        case "4":
            Console.WriteLine("Encerrando o sistema...");
            return;

        default:
            Console.WriteLine("Opção inválida. Tente novamente.");
            break;
    }
}
