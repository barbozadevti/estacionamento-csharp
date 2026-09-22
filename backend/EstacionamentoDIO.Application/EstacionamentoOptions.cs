namespace EstacionamentoDIO.Application;

public class EstacionamentoOptions
{
    public const string SectionName = "Estacionamento";

    public int VagasTotais { get; set; } = 20;
    public decimal PrecoInicial { get; set; } = 5.00m;
    public decimal PrecoPorHora { get; set; } = 2.00m;
}
