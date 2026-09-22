namespace EstacionamentoDIO.Application;

public class EstabelecimentoOptions
{
    public const string SectionName = "Estabelecimento";

    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
}
