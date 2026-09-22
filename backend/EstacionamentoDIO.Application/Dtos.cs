namespace EstacionamentoDIO.Application;

public record VeiculoDto(
    int Id,
    string Placa,
    DateTime HoraEntrada,
    DateTime? HoraSaida,
    decimal? ValorCobrado);

public record StatusEstacionamentoDto(
    int VagasTotais,
    int VagasDisponiveis,
    decimal PrecoInicial,
    decimal PrecoPorHora);

public record RegistrarSaidaResultDto(
    VeiculoDto Veiculo,
    decimal ValorCobrado,
    int Horas);
