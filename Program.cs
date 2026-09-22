using EstacionamentoDIO;

var estacionamento = new Estacionamento(precoInicial: 5.00m, precoPorHora: 2.00m);

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Sistema de Estacionamento ===");
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
            estacionamento.AdicionarVeiculo();
            break;
        case "2":
            estacionamento.RemoverVeiculo();
            break;
        case "3":
            estacionamento.ListarVeiculos();
            break;
        case "4":
            Console.WriteLine("Encerrando o sistema...");
            return;
        default:
            Console.WriteLine("Opção inválida. Tente novamente.");
            break;
    }
}
